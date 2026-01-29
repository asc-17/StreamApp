using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StreamApp.Models;

namespace StreamApp.Services
{
    public class MediaScannerService
    {
        private readonly string _mediaRootPath;
        private readonly ILogger<MediaScannerService> _logger;
        public List<Movie> Movies { get; private set; } = new List<Movie>();
        public List<Series> SeriesList { get; private set; } = new List<Series>();

        public MediaScannerService(IConfiguration configuration, ILogger<MediaScannerService> logger)
        {
            _mediaRootPath = configuration["MediaRootPath"] ?? "E:\\Media";
            _logger = logger;
        }

        public void ScanMedia()
        {
            _logger.LogInformation("Starting Scan from {Path}", _mediaRootPath);

            Movies.Clear();
            SeriesList.Clear();

            if (!Directory.Exists(_mediaRootPath))
            {
                _logger.LogWarning("Media root path not found: {Path}", _mediaRootPath);
                return;
            }

            ScanMovies(Path.Combine(_mediaRootPath, "Movies"));
            ScanSeries(Path.Combine(_mediaRootPath, "Series"));
            
            _logger.LogInformation("Scan Complete. Found {MovieCount} Movies, {SeriesCount} Series", Movies.Count, SeriesList.Count);
        }

        private void ScanMovies(string moviesPath)
        {
            if (!Directory.Exists(moviesPath)) return;

            var movieDirs = Directory.GetDirectories(moviesPath);
            foreach (var dir in movieDirs)
            {
                var dirName = Path.GetFileName(dir);
                // Expected format: "Movie Name (Year)"
                // But we act leniently.
                
                var mkvFiles = Directory.GetFiles(dir, "*.mkv");
                if (mkvFiles.Length == 0) continue;

                // Pick the largest MKV usually the movie, or the first one.
                // Assuming one movie file per folder as per structure.
                var movieFile = mkvFiles.OrderByDescending(f => new FileInfo(f).Length).FirstOrDefault();
                if (movieFile == null) continue;

                var posterPath = Directory.GetFiles(dir, "poster.jpg").FirstOrDefault();

                // Extract Year if present
                var yearMatch = Regex.Match(dirName, @"\((\d{4})\)");
                int year = yearMatch.Success ? int.Parse(yearMatch.Groups[1].Value) : 0;
                string title = yearMatch.Success ? dirName.Substring(0, yearMatch.Index).Trim() : dirName;

                Movies.Add(new Movie
                {
                    Title = title,
                    Year = year,
                    FilePath = movieFile,
                    FullPath = dir,
                    PosterPath = posterPath
                });
            }
        }

        private void ScanSeries(string seriesPath)
        {
            if (!Directory.Exists(seriesPath)) return;

            var seriesDirs = Directory.GetDirectories(seriesPath);
            foreach (var dir in seriesDirs)
            {
                var seriesName = Path.GetFileName(dir);
                var posterPath = Directory.GetFiles(dir, "poster.jpg").FirstOrDefault();
                
                var series = new Series
                {
                    Title = seriesName,
                    FullPath = dir,
                    PosterPath = posterPath
                };

                var seasonDirs = Directory.GetDirectories(dir, "Season *");
                foreach (var seasonDir in seasonDirs)
                {
                    var seasonName = Path.GetFileName(seasonDir);
                    var seasonMatch = Regex.Match(seasonName, @"Season\s+(\d+)");
                    int seasonNum = seasonMatch.Success ? int.Parse(seasonMatch.Groups[1].Value) : 0;

                    var season = new Season
                    {
                        Title = seasonName,
                        SeasonNumber = seasonNum
                    };

                    var episodeFiles = Directory.GetFiles(seasonDir, "*.mkv");
                    foreach (var epFile in episodeFiles)
                    {
                        var epName = Path.GetFileNameWithoutExtension(epFile);
                        // Try extract episode number "Episode 01"
                        var epMatch = Regex.Match(epName, @"Episode\s+(\d+)"); // Simple check
                        if (!epMatch.Success) epMatch = Regex.Match(epName, @"E(\d+)"); // fallback
                        
                        int epNum = epMatch.Success ? int.Parse(epMatch.Groups[1].Value) : 0;

                        season.Episodes.Add(new Episode
                        {
                            Title = epName,
                            FilePath = epFile,
                            EpisodeNumber = epNum
                        });
                    }
                    
                    if (season.Episodes.Count > 0)
                    {
                        season.Episodes = season.Episodes.OrderBy(e => e.EpisodeNumber).ToList();
                        series.Seasons.Add(season);
                    }
                }

                if (series.Seasons.Count > 0)
                {
                    series.Seasons = series.Seasons.OrderBy(s => s.SeasonNumber).ToList();
                    SeriesList.Add(series);
                }
            }
        }
        
        public Movie? GetMovie(string title) {
            return Movies.FirstOrDefault(m => m.Title == title || m.FullPath.EndsWith(title)); // Simple lookup
        }

        // Helper to find playing file by some ID logic if needed, or we just pass paths insecurely (but simple for local app)
        // For security better would indeed be hashes or cached IDs, but local app constraints: "Security: Ignore for now".
        // Base64 encode paths in URL is a common trick.
    }
}
