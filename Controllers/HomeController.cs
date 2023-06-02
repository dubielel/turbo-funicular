using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using turbo_funicular.Data;
using turbo_funicular.Models;

namespace turbo_funicular.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        if(HttpContext.Session.Keys.Contains("userId"))
            ViewData["userId"] = (int) HttpContext.Session.GetInt32("userId");
        return View();
    }

    public IActionResult Privacy()
    {
        if(HttpContext.Session.Keys.Contains("userId"))
            ViewData["userId"] = (int) HttpContext.Session.GetInt32("userId");
        return View();
    }

    public IActionResult PermissionDenied()
    {
        if(HttpContext.Session.Keys.Contains("userId"))
            ViewData["userId"] = (int) HttpContext.Session.GetInt32("userId");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
