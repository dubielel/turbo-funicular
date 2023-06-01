using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            var eventContext = _dbContext.Events.Include(e => e.User);
            return View(await eventContext.ToListAsync());
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

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

            return View(@event);
        }

        // GET: Event/Create
        public IActionResult Create()
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

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


            var userId = (int)HttpContext.Session.GetInt32("userId");
            var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId);
            var createDate = DateTime.Now;
                
            if (true)
            {
                _dbContext.Events.Add(new Event()
                    {
                        UserId = userId,
                        User = user,
                        Name = @event.Name,
                        Description = @event.Description,
                        CreateDate = createDate,
                        MaxParticipants = @event.MaxParticipants
                });
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_dbContext.Users, "Id", "Id", userId);
            return View(@event);
        }

        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (id == null || _dbContext.Events == null)
            {
                return NotFound();
            }

            var @event = await _dbContext.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_dbContext.Users, "Id", "Id", @event.UserId);
            return View(@event);
        }

        // POST: Event/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Name,Description,CreateDate,MaxParticipants")] Event @event)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Events.Update(@event);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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
            ViewData["UserId"] = new SelectList(_dbContext.Users, "Id", "Id", @event.UserId);
            return View(@event);
        }

        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

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
            var user = _dbContext.Events.FirstOrDefault(m => m.Name == name);
            return user == null;
        }
    }
}
