using System;
using System.ComponentModel.DataAnnotations;

namespace StreamApp.Models
{
    public class WatchProgress
    {
        public int Id { get; set; }
        
        [Required]
        public string FilePath { get; set; } = "";
        
        public double TimestampSeconds { get; set; }
        
        public DateTime LastWatched { get; set; } = DateTime.UtcNow;
    }
}
