using System;
using System.Collections.Generic;

namespace StreamApp.Models
{
    public enum MediaType
    {
        Movie,
        Series
    }

    public abstract class MediaItem
    {
        public string Title { get; set; } = string.Empty;
        public string? PosterPath { get; set; }
        public string FullPath { get; set; } = string.Empty; // For folders or files
    }

    public class Movie : MediaItem
    {
        public string FilePath { get; set; } = string.Empty;
        public int Year { get; set; }
    }

    public class Series : MediaItem
    {
        public List<Season> Seasons { get; set; } = new List<Season>();
    }

    public class Season
    {
        public string Title { get; set; } = string.Empty; // e.g. "Season 1"
        public int SeasonNumber { get; set; }
        public List<Episode> Episodes { get; set; } = new List<Episode>();
    }

    public class Episode
    {
        public string Title { get; set; } = string.Empty; // e.g. "Episode 01"
        public string FilePath { get; set; } = string.Empty;
        public int EpisodeNumber { get; set; }
    }
}
