using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using turbo_funicular.Data;
using turbo_funicular.Models;

namespace turbo_funicular.Controllers
{
    public class UserGroupController : Controller
    {
        private readonly ILogger<EventController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public UserGroupController(ILogger<EventController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET: Event
        public async Task<IActionResult> Join(int? groupId)
        {   
            if (!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");
            
            var userId = (int) HttpContext.Session.GetInt32("userId");
            var @group = await _dbContext.Groups.FirstOrDefaultAsync(e => e.Id == groupId);
            
            if (groupId == null || @group == null) {
                return NotFound();
            }

            if (!(await _dbContext.UserGroups.FirstOrDefaultAsync(
                            e => (e.UserId == userId && e.GroupId == groupId)) == null))
            {
                ModelState.AddModelError(string.Empty, "User already in group");
                return RedirectToRoute("GroupDetails", new { id = @group.Id });;
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId);
            var userGroup = new UserGroup()
            {
                UserId = userId,
                GroupId = (int)groupId,
                User = user,
                Group = group,
                IsModerator = false,
            };

            _dbContext.UserGroups.Add(userGroup);
            await _dbContext.SaveChangesAsync();
            return RedirectToRoute("GroupDetails", new { id = @group.Id });;
        }

        public async Task<IActionResult> Leave(int? groupId)
        {
            if (!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");
            
            var userId = (int) HttpContext.Session.GetInt32("userId");
            var @group = await _dbContext.Groups.FirstOrDefaultAsync(e => e.Id == groupId);
            
            if (groupId == null || @group == null) {
                return NotFound();
            }

            var userGroup = await _dbContext.UserGroups.FirstOrDefaultAsync(
                            e => (e.UserId == userId && e.GroupId == groupId));

            if ( userGroup == null)
            {
                ModelState.AddModelError(string.Empty, "User not in group");
                return RedirectToRoute("GroupDetails", new { id = @group.Id });;
            }

            if (@group.User.Id == userId)
            {
                ModelState.AddModelError(string.Empty, "Host cannot leave their group");
                return RedirectToRoute("GroupDetails", new { id = @group.Id });;
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId);

            if (@group != null)
            {
                _dbContext.UserGroups.Remove(userGroup);
            }
            
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Group");
        }
    }
}
