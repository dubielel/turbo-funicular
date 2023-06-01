using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using turbo_funicular.Data;
using turbo_funicular.Models;

namespace turbo_funicular.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public AccountController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public IActionResult Login()
    {
        if(HttpContext.Session.Keys.Contains("username"))
            return RedirectToAction("Index", "Home");
        
        return View();
    }

    [HttpPost] 
    public IActionResult Login(LoginViewModel model)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Username == model.Username);
        
        if (user != null && user.VerifyPassword(model.Password))
        {
            HttpContext.Session.SetInt32("userId", user.Id);
            return RedirectToAction("Index", "Home");
        }
        else 
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return View();
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
