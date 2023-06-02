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
        // public IActionResult Create()
        // {   
        //     if(!HttpContext.Session.Keys.Contains("userId"))
        //         return RedirectToAction("Login", "Account");

        //     ViewData["GroupId"] = new SelectList(_dbContext.Groups, "Id", "Id");
        //     ViewData["UserId"] = new SelectList(_dbContext.Users, "Id", "Id");
        //     return View();
        // }

        // POST: Message/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MessageView message)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");

            if (message.GroupId == null)
            {
                return NotFound();
            }

            var userId = (int) HttpContext.Session.GetInt32("userId");
            var @group = await _dbContext.Groups.FirstOrDefaultAsync(m => m.Id == message.GroupId);
            var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId);

            if (!user.isInGroup((int)message.GroupId))
            {
                return RedirectToAction("PermissionDenied", "Home");
            }

            var newMessage = new Message()
            {
                UserId = userId,
                GroupId = message.GroupId,
                User = user,
                Group = @group,
                CreateDate = DateTime.Now,
                Content = message.Content
            };

            user.Messages.Add(newMessage);
            @group.Messages.Add(newMessage);
            _dbContext.Messages.Add(newMessage);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Details", "Group", new {groupId = @group.Id});
        }

        private bool MessageExists(int id)
        {
          return (_dbContext.Messages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
