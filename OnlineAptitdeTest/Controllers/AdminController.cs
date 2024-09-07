using Microsoft.AspNetCore.Mvc;
using OnlineAptitdeTest.Data;
using OnlineAptitdeTest.Models.ViewModels;
using OnlineAptitdeTest.Models;
using Microsoft.EntityFrameworkCore;

namespace OnlineAptitdeTest.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {               

            int candidateCount = await _context.Candidates.CountAsync();
            int testCount = await _context.Tests.CountAsync();
            int questionCount = await _context.Questions.CountAsync();

            ViewBag.CandidateCount = candidateCount;
            ViewBag.TestCount = testCount;
            ViewBag.QuestionCount = questionCount;

            return View();
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var authenticatedUser = AuthenticateUser(model.Email, model.Password);

                if (authenticatedUser != null && BCrypt.Net.BCrypt.Verify(model.Password, authenticatedUser.PasswordHash))
                {
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    HttpContext.Session.SetString("Username", authenticatedUser.Email);

                    var userRole = authenticatedUser.UserRoles.FirstOrDefault()?.Role?.Name;
                    if (userRole != null)
                    {
                        HttpContext.Session.SetString("UserRole", userRole);
                        HttpContext.Session.SetString("UserSession", "Active");
                    }

                    HttpContext.Session.SetString("UserSession", "Active");

                    _logger.LogInformation("Admin logged in successfully.");
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid credentials. Please try again.");
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            _logger.LogInformation("Admin logged out successfully.");

            return RedirectToAction("Login");
        }

        private Admin AuthenticateUser(string email, string password)
        {
            var userFromDb = _context.Admins
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.Email == email);



            if (userFromDb != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, userFromDb.PasswordHash))
                {
                    return userFromDb; 
                }
            }

            return null; 
        }

        [HttpGet]
        public IActionResult CreateManager()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateManager(RegisterViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "The password and confirmation password do not match.");
                return View(model);
            }

            var existingManager = await _context.Managers.FirstOrDefaultAsync(m => m.Email == model.Email);
            if (existingManager != null)
            {
                ModelState.AddModelError(nameof(model.Email), "The email address is already in use.");
                return View(model);
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var manager = new Manager
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = hashedPassword
            };

            if (ModelState.IsValid)
            {
                await EnsureRolesExist();

                var managerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Manager");
                if (managerRole != null)
                {

                    var userRole = new UserRole
                    {
                        UserId = manager.Id, 
                        RoleId = managerRole.Id
                    };

                    _context.Managers.Add(manager);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ManageManagers");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The Manager role does not exist.");
                }
            }

            return View(model);
        }

        private async Task EnsureRolesExist()
        {
            if (!_context.Roles.Any(r => r.Name == "Manager"))
            {
                _context.Roles.Add(new Role { Name = "Manager" });
                await _context.SaveChangesAsync();
                _logger.LogInformation("Manager role created.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteManager(int id)
        {
            var manager = await _context.Managers
                .Include(m => m.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manager == null)
            {
                return NotFound();
            }

            return View(manager);
        }

        [HttpPost, ActionName("DeleteManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var manager = await _context.Managers.FindAsync(id);
            if (manager == null)
            {
                return NotFound();
            }

            var userRoles = _context.UserRoles.Where(ur => ur.Managers.Id == id);
            _context.UserRoles.RemoveRange(userRoles);

            _context.Managers.Remove(manager);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageManagers");
        }


        [HttpGet]
        public IActionResult ManageManagers()
        {
            var managers = _context.Managers.ToList();
            return View(managers);
        }


    }
}
