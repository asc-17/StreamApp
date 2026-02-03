using Microsoft.AspNetCore.Mvc;
using StreamApp.Services;
using System.IO;
using System.Collections.Generic;

namespace StreamApp.Controllers
{
    [Route("[controller]")]
    public class VideoController : Controller
    {
        private readonly MediaScannerService _scanner;
        private readonly SubtitleService _subtitleService;

        public VideoController(MediaScannerService scanner, SubtitleService subtitleService)
        {
            _scanner = scanner;
            _subtitleService = subtitleService;
        }

        [HttpGet]
        [Route("Stream")]
        public IActionResult Stream(string path)
        {
            if (string.IsNullOrEmpty(path)) return BadRequest();
            
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            
            string mimeType = "video/x-matroska";
            if (path.EndsWith(".mp4")) mimeType = "video/mp4";

            return File(stream, mimeType, enableRangeProcessing: true);
        }

        [HttpGet]
        [Route("Image")]
        public IActionResult Image(string path)
        {
            if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path)) return NotFound();
            
            string mime = "image/jpeg";
            if (path.EndsWith(".png")) mime = "image/png";
            
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, mime);
        }

        [HttpGet]
        [Route("Play")]
        public IActionResult Play(string path)
        {
             if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path)) return NotFound();
             
             ViewBag.VideoPath = Url.Action("Stream", "Video", new { path = path });
             ViewBag.Title = Path.GetFileNameWithoutExtension(path);
             
             // Look for subtitle files in Subtitles folder next to video
             var directory = Path.GetDirectoryName(path);
             var fileName = Path.GetFileNameWithoutExtension(path);
             var subtitles = new List<string>();
             
             if (!string.IsNullOrEmpty(directory))
             {
                 // Check for Subtitles subfolder
                 var subtitlesFolder = Path.Combine(directory, "Subtitles");
                 if (Directory.Exists(subtitlesFolder))
                 {
                     // Look for subtitle files with same name as video
                     var srtFile = Path.Combine(subtitlesFolder, $"{fileName}.srt");
                     var vttFile = Path.Combine(subtitlesFolder, $"{fileName}.vtt");
                     
                     if (System.IO.File.Exists(srtFile)) subtitles.Add(srtFile);
                     if (System.IO.File.Exists(vttFile)) subtitles.Add(vttFile);
                 }
             }
             
             ViewBag.Subtitles = subtitles;
             ViewBag.VideoFilePath = path; // Important for Progress tracking
             return View();
        }

        [HttpGet]
        [Route("Subtitle")]
        public IActionResult Subtitle(string path)
        {
            if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path)) return NotFound();
            
            string mimeType = "text/plain";
            if (path.EndsWith(".vtt")) mimeType = "text/vtt";
            else if (path.EndsWith(".srt")) mimeType = "application/x-subrip";
            
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, mimeType);
        }

        [HttpGet]
        [Route("Subtitles")]
        public async Task<IActionResult> GetVideoSubtitles(string path)
        {
             if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path)) return NotFound();

             // 1. Internal MKV Subtitles
             var internalTracks = await _subtitleService.GetSubtitleTracksAsync(path);

             // 2. External Subtitles
             var externalTracks = new List<object>();
             var directory = Path.GetDirectoryName(path);
             var fileName = Path.GetFileNameWithoutExtension(path);
             
             if (!string.IsNullOrEmpty(directory))
             {
                 var subtitlesFolder = Path.Combine(directory, "Subtitles");
                 if (Directory.Exists(subtitlesFolder))
                 {
                     var srtFile = Path.Combine(subtitlesFolder, $"{fileName}.srt");
                     var vttFile = Path.Combine(subtitlesFolder, $"{fileName}.vtt");
                     
                     if (System.IO.File.Exists(srtFile)) externalTracks.Add(new { type = "external", path = srtFile, language = "External (SRT)", title = "Local File" });
                     if (System.IO.File.Exists(vttFile)) externalTracks.Add(new { type = "external", path = vttFile, language = "External (VTT)", title = "Local File" });
                 }
             }

             return Json(new { internalMatches = internalTracks, externalMatches = externalTracks });
        }

        [HttpGet]
        [Route("StreamSubtitle")]
        public IActionResult StreamSubtitle(string path, int index)
        {
             if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path)) return NotFound();
             
             var stream = _subtitleService.ExtractSubtitleStream(path, index);
             if (stream == Stream.Null) return NotFound();
             
             return File(stream, "text/vtt");
        }
    }
}
