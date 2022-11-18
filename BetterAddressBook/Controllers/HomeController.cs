using System.Diagnostics;
using BetterAddressBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BetterAddressBook.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

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
        return View(
            new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            }
        );
    }

    [Route("/Home/HandleError/{code:int}")]
    public IActionResult HandleError(int code)
    {
        CustomError model = new()
        {
            Code = code,
            Message = code switch
            {
                404 => "Page now found. please return to the home page",
                500 => "Internal Server Error. please return to the home page",
                _ => "Unknown error occured. Please try again later"
            },
            ViewPath = code switch
            {
                404 => "~/Views/Home/CustomErrors/404ErrorPage.cshtml",
                500 => "~/Views/Home/CustomErrors/500ErrorPage.cshtml",
                _ => "~/Views/Home/Shared/Error.cshtml"
            }
        };
        
        return View(model.ViewPath, model);
    }
}