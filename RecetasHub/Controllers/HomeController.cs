using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecetasHub.Models;

namespace RecetasHub.Controllers;

[Authorize] 
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult MisRecetas()
    {
        return View();
    }

    [AllowAnonymous] // para que privacy puede verla cualquiera
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}