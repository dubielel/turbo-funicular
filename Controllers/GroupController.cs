using System.Collections.ObjectModel;
using System.Data.Common;
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
        public async Task<IActionResult> Create(GroupViewModel @group)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            var userId = (int)HttpContext.Session.GetInt32("userId");
            var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId);
            var createTime = DateTime.Now;

            if (!VerifyUniqueGroupName(group.Name))
            {
                ModelState.AddModelError(string.Empty, "Name already in use");
                return View();
            }

            var newGroup = new Group() 
                {
                    UserId = userId,
                    User = user,
                    Name = @group.Name,
                    Description = @group.Description,
                    CreateTime = createTime,
                    UpdateTime = createTime
                };

            user.OwnedGroups.Add(newGroup);
            _dbContext.Groups.Add(newGroup);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
            var userId = (int) HttpContext.Session.GetInt32("userId");

            if (@group.UserId != userId)
            {
                return RedirectToAction("PermissionDenied", "Home");
            }

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
        public async Task<IActionResult> Edit(int id, GroupViewModel @group)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (!VerifyUniqueGroupName(@group.Name))
            {
                ModelState.AddModelError(string.Empty, "Name already in use");
                return View();
            }

            var updatedGroup = await _dbContext.Groups.FirstOrDefaultAsync(e => e.Id == id);

            if (updatedGroup == null)
            {
                return NotFound();
            }

            var userId = (int) HttpContext.Session.GetInt32("userId");

            if (updatedGroup.UserId != userId)
            {
                ModelState.AddModelError(string.Empty, "Only event creator can edit event");
                return RedirectToAction(nameof(Index));
            }

            updatedGroup.Name = @group.Name;
            updatedGroup.Description = @group.Description;

            try
            {
                _dbContext.Groups.Update(updatedGroup);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(updatedGroup.Id))
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

        // GET: Group/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (id == null || _dbContext.Groups == null)
            {
                return NotFound();
            }

            var userId = (int) HttpContext.Session.GetInt32("userId");
            var @group = await _dbContext.Groups
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@group.UserId != userId)
            {
                return RedirectToAction("PermissionDenied", "Home");
            }

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
            var userId = (int) HttpContext.Session.GetInt32("userId");
            if (@group.UserId != userId)
            {
                return RedirectToAction("PermissionDenied", "Home");
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId);
            user.OwnedGroups.Remove(@group);

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
