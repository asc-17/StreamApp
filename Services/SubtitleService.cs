using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Caching.Memory;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace StreamApp.Services
{
    public class SubtitleService
    {
        private readonly ILogger<SubtitleService> _logger;
        private readonly IMemoryCache _cache;
        private readonly string _ffprobePath;
        private readonly string _ffmpegPath;

        public SubtitleService(ILogger<SubtitleService> logger, IMemoryCache cache, IConfiguration configuration)
        {
            _logger = logger;
            _cache = cache;
            _ffprobePath = configuration["FFprobePath"] ?? "ffprobe";
            _ffmpegPath = configuration["FFmpegPath"] ?? "ffmpeg";
        }

        public class SubtitleTrack
        {
            public int Index { get; set; }
            public string Language { get; set; } = "Unknown";
            public string Title { get; set; }
            public string Codec { get; set; }
        }

        public async Task<List<SubtitleTrack>> GetSubtitleTracksAsync(string filePath)
        {
            const int CacheDurationMinutes = 60 * 24; // Cache for 24 hours
            
            return await _cache.GetOrCreateAsync($"subtitle_tracks_{filePath.GetHashCode()}", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(CacheDurationMinutes);
                
                var tracks = new List<SubtitleTrack>();
                try
                {
                var startInfo = new ProcessStartInfo
                {
                    FileName = _ffprobePath,
                    Arguments = $"-v error -select_streams s -show_entries stream=index,codec_name:stream_tags=language,title -of json \"{filePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = startInfo };
                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    _logger.LogWarning("ffprobe exited with code {Code}. Error: {Error}. Arguments: {Args}", process.ExitCode, error, startInfo.Arguments);
                }

                if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
                {
                    var json = JsonDocument.Parse(output);
                    if (json.RootElement.TryGetProperty("streams", out var streams))
                    {
                        foreach (var stream in streams.EnumerateArray())
                        {
                            var track = new SubtitleTrack
                            {
                                Index = stream.GetProperty("index").GetInt32(),
                                Codec = stream.TryGetProperty("codec_name", out var codec) ? codec.GetString() : "unknown"
                            };

                            if (stream.TryGetProperty("tags", out var tags))
                            {
                                if (tags.TryGetProperty("language", out var lang)) track.Language = lang.GetString();
                                if (tags.TryGetProperty("title", out var title)) track.Title = title.GetString();
                            }

                            tracks.Add(track);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error probing subtitles for {Path}", filePath);
            }

            return tracks;
            });
        }

        public Stream ExtractSubtitleStream(string filePath, int streamIndex)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    // -vn: no video, -an: no audio, -map 0:{streamIndex}: select specific stream, -f webvtt: output format
                    Arguments = $"-i \"{filePath}\" -map 0:{streamIndex} -f webvtt -",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true, // We don't want stderr in stdout
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = new Process { StartInfo = startInfo };
                process.Start();

                // We return the StandardOutput stream directly. 
                // Note: The caller is responsible for disposing the stream, which will close the pipe.
                // However, the process object itself might linger if not managed. 
                // meaningful stream processing usually dominates the lifespan. 
                // For a robust implementation, we might want a wrapper stream that kills the process on Close().
                
                return process.StandardOutput.BaseStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting subtitle stream {Index} from {Path}", streamIndex, filePath);
                return Stream.Null;
            }
        }
    }
}
