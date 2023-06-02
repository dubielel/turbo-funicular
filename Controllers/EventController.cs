using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using turbo_funicular.Data;
using turbo_funicular.Models;

namespace turbo_funicular.Controllers
{
    public class EventController : Controller
    {
        private readonly ILogger<EventController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public EventController(ILogger<EventController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET: Event
        public async Task<IActionResult> Index()
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            ViewData["userId"] = (int) HttpContext.Session.GetInt32("userId");

            var eventContext = _dbContext.Events.Include(e => e.User);
            foreach (var @event in await eventContext.ToListAsync())
            {
                @event.UserEvents = @event.GetUserEvents(_dbContext);
            }
            return View(await eventContext.ToListAsync());
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");
            
            ViewData["userId"] = (int) HttpContext.Session.GetInt32("userId");

            if (id == null || _dbContext.Events == null)
            {
                return NotFound();
            }

            var @event = await _dbContext.Events
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            @event.UserEvents = @event.GetUserEvents(_dbContext);
            return View(@event);
        }

        // GET: Event/Create
        public IActionResult Create()
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            ViewData["userId"] = (int) HttpContext.Session.GetInt32("userId");

            ViewData["UserId"] = new SelectList(_dbContext.Users, "Id", "Id");
            return View();
        }

        // POST: Event/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventViewModel @event)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            var userId = (int) HttpContext.Session.GetInt32("userId");
            var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId);
            var createDate = DateTime.Now;

            if (!VerifyUniqueEventName(@event.Name))
            {
                ModelState.AddModelError(string.Empty, "Name already in use");
                return View();
            }

            if (!VerifyPositiveMaxParticipants(@event.MaxParticipants))
            {
                ModelState.AddModelError(string.Empty, "Maximum Number of participants should be greater than 0");
                return View();
            }

            var newEvent = new Event()
                {
                    UserId = userId,
                    User = user,
                    Name = @event.Name,
                    Description = @event.Description,
                    CreateDate = createDate,
                    MaxParticipants = @event.MaxParticipants
            };

            _dbContext.Events.Add(newEvent);

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Join", "UserEvent", new {eventId = newEvent.Id});;
        }

        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            ViewData["userId"] = (int) HttpContext.Session.GetInt32("userId");

            if (id == null || _dbContext.Events == null)
            {
                return NotFound();
            }

            var @event = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == id);
            var userId = (int) HttpContext.Session.GetInt32("userId");

            if (@event.UserId != userId)
            {
                return RedirectToAction("PermissionDenied", "Home");
            }

            if (@event == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_dbContext.Users, "Id", "Id", @event.UserId);
            @event.UserEvents = @event.GetUserEvents(_dbContext);
            return View(@event);
        }

        // POST: Event/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventViewModel @event)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            var updatedEvent = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == id);

            if (updatedEvent == null)
            {
                return NotFound();
            }

            if (!VerifyUniqueEventName(@event.Name))
            {
                ModelState.AddModelError(string.Empty, "Name already in use");
                return View();
            }

            if (!VerifyPositiveMaxParticipants(@event.MaxParticipants))
            {
                ModelState.AddModelError(string.Empty, "Maximum Number of participants");
                return View();
            }

            var userId = (int) HttpContext.Session.GetInt32("userId");

            if (updatedEvent.UserId != userId)
            {
                ModelState.AddModelError(string.Empty, "Only event creator can edit event");
                return RedirectToAction(nameof(Index));
            }

            updatedEvent.Name = @event.Name;
            updatedEvent.Description = @event.Description;
            updatedEvent.MaxParticipants = @event.MaxParticipants;

            try
            {
                _dbContext.Events.Update(updatedEvent);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");
            
            ViewData["userId"] = (int) HttpContext.Session.GetInt32("userId");

            if (id == null || _dbContext.Events == null)
            {
                return NotFound();
            }

            var userId = (int) HttpContext.Session.GetInt32("userId");
            var @event = await _dbContext.Events
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event.UserId != userId)
            {
                return RedirectToAction("PermissionDenied", "Home");
            }

            if (@event == null)
            {
                return NotFound();
            }

            @event.UserEvents = @event.GetUserEvents(_dbContext);
            return View(@event);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (_dbContext.Events == null)
            {
                return Problem("Entity set 'EventContext.Event'  is null.");
            }

            var @event = await _dbContext.Events.FindAsync(id);
            var userId = (int) HttpContext.Session.GetInt32("userId");
            if (@event.UserId != userId)
            {
                return RedirectToAction("PermissionDenied", "Home");
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId);

            foreach (var userEvent in @event.UserEvents)
            {
                userEvent.User.UserEvents.Remove(userEvent);
                @event.UserEvents.Remove(userEvent);
                if (@event != null)
                {
                    _dbContext.UserEvents.Remove(userEvent);
                }
                
                await _dbContext.SaveChangesAsync();
            }

            if (@event != null)
            {
                _dbContext.Events.Remove(@event);
            }
            
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
          return (_dbContext.Events?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool VerifyUniqueEventName(string name) 
        {
            var @event = _dbContext.Events.FirstOrDefault(m => m.Name == name);
            return @event == null;
        }

        private bool VerifyPositiveMaxParticipants(int maxParticipants)
        {
            return maxParticipants > 0;
        }
    }
}
