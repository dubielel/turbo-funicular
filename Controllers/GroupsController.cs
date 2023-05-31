using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using turbo_funicular.Models;

namespace turbo_funicular.Controllers;

public class GroupsController : Controller
{
    private readonly ILogger<GroupsController> _logger;

    public GroupsController(ILogger<GroupsController> logger)
    {
        _logger = logger;
    }

    public IActionResult Overview()
    {
        return View();
    }
}