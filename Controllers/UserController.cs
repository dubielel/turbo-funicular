using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using turbo_funicular.Data;
using turbo_funicular.Models;

namespace turbo_funicular.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public UserController(ILogger<UserController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IActionResult Overview()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        
        if (ModelState.IsValid)
        {   
            user.SetPassword(user.PasswordHash);
            user.CreateDate = DateTime.Now;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        return View(_dbContext);
    }
}