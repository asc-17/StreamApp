using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamApp.Data;
using StreamApp.Models;
using System;
using System.Threading.Tasks;

namespace StreamApp.Controllers
{
    [Route("[controller]")]
    public class ProgressController : Controller
    {
        private readonly AppDbContext _context;

        public ProgressController(AppDbContext context)
        {
            _context = context;
        }

        public class ProgressUpdate
        {
            public string Path { get; set; } = "";
            public double Time { get; set; }
        }

        [HttpPost]
        [Route("Update")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Update([FromBody] ProgressUpdate update)
        {
            Console.WriteLine($"Progress Update Received. Path: {update?.Path}, Time: {update?.Time}");
            if (string.IsNullOrEmpty(update?.Path)) return BadRequest("Path is null or empty");

            var progress = await _context.WatchProgresses.FirstOrDefaultAsync(w => w.FilePath == update.Path);
            
            if (progress == null)
            {
                progress = new WatchProgress
                {
                    FilePath = update.Path,
                    TimestampSeconds = update.Time,
                    LastWatched = DateTime.UtcNow
                };
                _context.WatchProgresses.Add(progress);
            }
            else
            {
                progress.TimestampSeconds = update.Time;
                progress.LastWatched = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get(string path)
        {
            if (string.IsNullOrEmpty(path)) return BadRequest();

            var progress = await _context.WatchProgresses.AsNoTracking().FirstOrDefaultAsync(w => w.FilePath == path);
            
            return Json(new { time = progress?.TimestampSeconds ?? 0 });
        }
    }
}
