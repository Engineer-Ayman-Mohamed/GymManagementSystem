using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GymManagementSystem.PresentationLayer.Models;

namespace GymManagementSystem.PresentationLayer.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View(); 
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var model = new ErrorViewModel
        {
            RequestId = TempData["RequestId"] as string
                ?? Activity.Current?.Id
                ?? HttpContext.TraceIdentifier,
            StatusCode = int.TryParse(TempData["StatusCode"]?.ToString(), out var sc) ? sc : 0,
            ErrorMessage = TempData["ErrorMessage"] as string,
            ExceptionDetails = TempData["ExceptionDetails"] as string
        };

        if (model.StatusCode > 0)
        {
            HttpContext.Response.StatusCode = model.StatusCode;
        }

        return View(model);
    }
}
