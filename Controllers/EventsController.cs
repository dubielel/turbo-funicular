using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using turbo_funicular.Models;

namespace turbo_funicular.Controllers;

public class EventsController : Controller
{
    private readonly ILogger<EventsController> _logger;

    public EventsController(ILogger<EventsController> logger)
    {
        _logger = logger;
    }

    public IActionResult Overview()
    {
        return View();
    }
}