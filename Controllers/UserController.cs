using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using turbo_funicular.Data;
using turbo_funicular.Models;

namespace turbo_funicular.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public UserController(ILogger<UserController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");
            
            ViewData["userId"] = (int) HttpContext.Session.GetInt32("userId");

            return _dbContext.Users != null ? 
                          View(await _dbContext.Users.ToListAsync()) :
                          Problem("Entity set 'UserContext.User'  is null.");
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");
            
            ViewData["userId"] = (int) HttpContext.Session.GetInt32("userId");

            if (id == null || _dbContext.Users == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");
            
            ViewData["userId"] = (int) HttpContext.Session.GetInt32("userId");

            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel user)
        {   
            if(!HttpContext.Session.Keys.Contains("userId"))
                return RedirectToAction("Login", "Account");
            
            if(HttpContext.Session.GetInt32("userId") != 1)
                return RedirectToAction("PermissionDenied", "Home");

            if (!VerifyUniqueUsername(user.Username))
            {
                ModelState.AddModelError(string.Empty, "Username already in use");
                return View();
            }

            var createDate = DateTime.Now;

            _logger.LogInformation(user.Password);

            var newUser = new User()
            {
                Username = user.Username,
                PasswordHash = "",
                CreateDate = createDate,  
            };
            newUser.SetPassword(user.Password);


            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_dbContext.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool VerifyUniqueUsername(string username) 
        {
            var user = _dbContext.Users.FirstOrDefault(m => m.Username == username);
            return user == null;
        }
    }
}
