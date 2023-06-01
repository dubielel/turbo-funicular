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
    public class MessageController : Controller
    {
        private readonly ILogger<MessageController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public MessageController(ILogger<MessageController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET: Message
        public async Task<IActionResult> Index()
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            var messageContext = _dbContext.Messages.Include(m => m.Group).Include(m => m.User);
            return View(await messageContext.ToListAsync());
        }

        // GET: Message/Details/5
        public async Task<IActionResult> Details(int? id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (id == null || _dbContext.Messages == null)
            {
                return NotFound();
            }

            var message = await _dbContext.Messages
                .Include(m => m.Group)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Message/Create
        public IActionResult Create()
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            ViewData["GroupId"] = new SelectList(_dbContext.Groups, "Id", "Id");
            ViewData["UserId"] = new SelectList(_dbContext.Users, "Id", "Id");
            return View();
        }

        // POST: Message/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,GroupId,Content,CreateDate")] Message message)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _dbContext.Messages.Add(message);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_dbContext.Groups, "Id", "Id", message.GroupId);
            ViewData["UserId"] = new SelectList(_dbContext.Users, "Id", "Id", message.UserId);
            return View(message);
        }

        // GET: Message/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (id == null || _dbContext.Messages == null)
            {
                return NotFound();
            }

            var message = await _dbContext.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(_dbContext.Groups, "Id", "Id", message.GroupId);
            ViewData["UserId"] = new SelectList(_dbContext.Users, "Id", "Id", message.UserId);
            return View(message);
        }

        // POST: Message/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,GroupId,Content,CreateDate")] Message message)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Messages.Update(message);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
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
            ViewData["GroupId"] = new SelectList(_dbContext.Groups, "Id", "Id", message.GroupId);
            ViewData["UserId"] = new SelectList(_dbContext.Users, "Id", "Id", message.UserId);
            return View(message);
        }

        // GET: Message/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (id == null || _dbContext.Messages == null)
            {
                return NotFound();
            }

            var message = await _dbContext.Messages
                .Include(m => m.Group)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Message/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (_dbContext.Messages == null)
            {
                return Problem("Entity set 'MessageContext.Message'  is null.");
            }
            var message = await _dbContext.Messages.FindAsync(id);
            if (message != null)
            {
                _dbContext.Messages.Remove(message);
            }
            
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
          return (_dbContext.Messages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
