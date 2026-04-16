using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcBankAccountSim.API.Models;

namespace MvcBankAccountSim.API.Controllers;

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
        return View(new ErrorViewModel { 
            Message = "A file system error occurred. Please try again later.",
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        });
    }
}
