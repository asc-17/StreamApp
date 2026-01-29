using Microsoft.AspNetCore.Mvc;
using StreamApp.Services;
using System.Linq;

namespace StreamApp.Controllers
{
    public class SeriesController : Controller
    {
        private readonly MediaScannerService _scanner;

        public SeriesController(MediaScannerService scanner)
        {
            _scanner = scanner;
        }

        [Route("Series/Details/{id}")]
        public IActionResult Details(string id)
        {
            var series = _scanner.SeriesList.FirstOrDefault(s => s.Title == id);
            if (series == null) return NotFound();
            return View(series);
        }
    }
}
