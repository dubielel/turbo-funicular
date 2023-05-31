using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using turbo_funicular.Data;
using turbo_funicular.Models;

namespace turbo_funicular.Controllers;

public class GroupsController : Controller
{
    private readonly ILogger<GroupsController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public GroupsController(ILogger<GroupsController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IActionResult Overview()
    {
        return View();
    }
}