using Microsoft.AspNetCore.Mvc;
using StreamApp.Services;
using System.IO;

namespace StreamApp.Controllers
{
    public class VideoController : Controller
    {
        private readonly MediaScannerService _scanner;

        public VideoController(MediaScannerService scanner)
        {
            _scanner = scanner;
        }

        [HttpGet]
        [Route("Video/Stream")]
        public IActionResult Stream(string path)
        {
            if (string.IsNullOrEmpty(path)) return BadRequest();

            // Security warning: In production, sanitize this path or map it to an ID!
            // For this local app, we trust the input (it will come from our own UI).
            // But we should verify it exists.
            
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            
            // "video/x-matroska" is the standard for mkv, but some browsers play better with "video/mp4" or "video/webm"
            // if the underlying stream is compatible. However, we'll send correct mime type.
            // If chrome fails to play mkv, it's a codec issue, not just mime type.
            string mimeType = "video/x-matroska";
            if (path.EndsWith(".mp4")) mimeType = "video/mp4";

            return File(stream, mimeType, enableRangeProcessing: true);
        }

        [HttpGet]
        [Route("Video/Image")]
        public IActionResult Image(string path)
        {
            if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path)) return NotFound();
            // Basic mime type detection
            string mime = "image/jpeg";
            if (path.EndsWith(".png")) mime = "image/png";
            
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, mime);
        }

        [HttpGet]
        [Route("Video/Play")]
        public IActionResult Play(string path)
        {
             if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path)) return NotFound();
             // Pass path to view
             ViewBag.VideoPath = Url.Action("Stream", "Video", new { path = path });
             // We might want to pass Title too, but path is enough for basic player
             ViewBag.Title = Path.GetFileNameWithoutExtension(path);
             return View();
        }
    }
}
