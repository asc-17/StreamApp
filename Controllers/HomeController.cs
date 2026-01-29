using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StreamApp.Models;
using StreamApp.Services;

namespace StreamApp.Controllers;

public class HomeController : Controller
{
    private readonly MediaScannerService _scanner;

    public HomeController(MediaScannerService scanner)
    {
        _scanner = scanner;
    }

    public IActionResult Index()
    {
        var model = new StreamApp.ViewModels.HomeViewModel
        {
            Movies = _scanner.Movies,
            Series = _scanner.SeriesList
        };
        return View(model);
    }

    public IActionResult Details(string id)
    {
        var movie = _scanner.GetMovie(id);
        if (movie == null) return NotFound();
        return View(movie);
    }

    /*
    public IActionResult Privacy()
    {
        return View();
    }
    */

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
