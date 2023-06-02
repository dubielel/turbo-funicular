using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using turbo_funicular.Data;
using turbo_funicular.Models;

namespace turbo_funicular.Controllers
{
    public class UserEventController : Controller
    {
        private readonly ILogger<EventController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public UserEventController(ILogger<EventController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET: Event
        public async Task<IActionResult> Join(int? eventId)
        {   
            if (!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");
            
            var userId = (int) HttpContext.Session.GetInt32("userId");
            var @event = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            
            if (eventId == null || @event == null) {
                return NotFound();
            }

            if (!(await _dbContext.UserEvents.FirstOrDefaultAsync(
                            e => (e.UserId == userId && e.EventId == eventId)) == null))
            {
                ModelState.AddModelError(string.Empty, "User already in event");
                return RedirectToRoute("EventDetails", new { id = @event.Id });
            }

            var numberOfParticipants = await _dbContext.UserEvents.CountAsync(e => e.EventId == eventId);
            if (numberOfParticipants >= @event.MaxParticipants)
            {
                ModelState.AddModelError(string.Empty, "Maximum number of participants has been reached");
                return RedirectToAction("Index", "Event");
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId);
            var userEvent = new UserEvent()
            {
                UserId = userId,
                EventId = (int)eventId,
                User = user,
                Event = @event
            };

            user.UserEvents.Add(userEvent);
            @event.UserEvents.Add(userEvent);
            _dbContext.UserEvents.Add(userEvent);

            await _dbContext.SaveChangesAsync();
            return RedirectToRoute("EventDetails", new { id = @event.Id });
        }

        public async Task<IActionResult> Leave(int? eventId)
        {
            if (!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");
            
            var userId = (int) HttpContext.Session.GetInt32("userId");
            var @event = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            
            if (eventId == null || @event == null) {
                return NotFound();
            }

            var userEvent = await _dbContext.UserEvents.FirstOrDefaultAsync(
                            e => (e.UserId == userId && e.EventId == eventId));

            if (userEvent == null)
            {
                ModelState.AddModelError(string.Empty, "User not in event");
                return RedirectToRoute("EventDetails", new { id = @event.Id });
            }

            if (@event.UserId == userId)
            {
                ModelState.AddModelError(string.Empty, "Host cannot leave their event");
                return RedirectToRoute("EventDetails", new { id = @event.Id });
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId);
            user.UserEvents.Remove(userEvent);
            @event.UserEvents.Remove(userEvent);

            if (@event != null)
            {
                _dbContext.UserEvents.Remove(userEvent);
            }
            
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Event");
        }
    }
}
