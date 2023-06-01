using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using turbo_funicular.Models;
using turbo_funicular.Data;

namespace turbo_funicular.Controllers
{
    public class GroupController : Controller
    {
        private readonly ILogger<GroupController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public GroupController(ILogger<GroupController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET: Group
        public async Task<IActionResult> Index()
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            var groupContext = _dbContext.Groups.Include(g => g.User);
            return View(await groupContext.ToListAsync());
        }

        // GET: Group/Details/5
        public async Task<IActionResult> Details(int? id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (id == null || _dbContext.Groups == null)
            {
                return NotFound();
            }

            var @group = await _dbContext.Groups
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // GET: Group/Create
        public IActionResult Create()
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            ViewData["UserId"] = new SelectList(_dbContext.Users, "Id", "Id");
            return View();
        }

        // POST: Group/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Name,Description,CreateTime,UpdateTime")] Group @group)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {   
                if (!VerifyUniqueGroupName(@group.Name))
                {
                    ModelState.AddModelError(string.Empty, "Name already in use");
                    return View();
                }
                
                _dbContext.Groups.Add(@group);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_dbContext.Users, "Id", "Id", @group.UserId);
            return View(@group);
        }

        // GET: Group/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (id == null || _dbContext.Groups == null)
            {
                return NotFound();
            }

            var @group = await _dbContext.Groups.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_dbContext.Users, "Id", "Id", @group.UserId);
            return View(@group);
        }

        // POST: Group/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Name,Description,CreateTime,UpdateTime")] Group @group)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (id != @group.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!VerifyUniqueGroupName(@group.Name))
                {
                    ModelState.AddModelError(string.Empty, "Name already in use");
                    return View();
                }

                try
                {
                    _dbContext.Groups.Update(@group);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.Id))
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
            ViewData["UserId"] = new SelectList(_dbContext.Users, "Id", "Id", @group.UserId);
            return View(@group);
        }

        // GET: Group/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (id == null || _dbContext.Groups == null)
            {
                return NotFound();
            }

            var @group = await _dbContext.Groups
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // POST: Group/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (_dbContext.Groups == null)
            {
                return Problem("Entity set 'GroupContext.Group'  is null.");
            }
            var @group = await _dbContext.Groups.FindAsync(id);
            if (@group != null)
            {
                _dbContext.Groups.Remove(@group);
            }
            
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {   
          return (_dbContext.Groups?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool VerifyUniqueGroupName(string name) 
        {
            var user = _dbContext.Groups.FirstOrDefault(m => m.Name == name);
            return user == null;
        }
    }
}
