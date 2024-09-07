using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using OnlineAptitdeTest.Models;
using OnlineAptitdeTest.Models.ViewModels;
using OnlineAptitdeTest.Data;
using BCrypt.Net;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: /Candidate/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Candidate/Login
    [HttpPost]
    public async Task<IActionResult> Login(CandidateLoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var authenticatedCandidate = AuthenticateCandidate(model.Username, model.Password);

            if (authenticatedCandidate != null && BCrypt.Net.BCrypt.Verify(model.Password, authenticatedCandidate.PasswordHash))
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("Username", authenticatedCandidate.Username);
                HttpContext.Session.SetString("CandidateId", authenticatedCandidate.CandidateId.ToString()); // Store CandidateId as string
                HttpContext.Session.SetString("UserSession", "Active");

                _logger.LogInformation("Candidate logged in successfully.");

                return RedirectToAction("Index", "Test");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials. Please try again.");
            }
        }

        return View(model);
    }

    // POST: /Candidate/Logout
    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        _logger.LogInformation("Candidate logged out successfully.");

        return RedirectToAction("Login");
    }

    private Candidate AuthenticateCandidate(string username, string password)
    {
        var candidateFromDb = _context.Candidates
            .FirstOrDefault(c => c.Username == username);

        if (candidateFromDb != null)
        {
            if (BCrypt.Net.BCrypt.Verify(password, candidateFromDb.PasswordHash))
            {
                return candidateFromDb;
            }
        }

        return null; 
    }
}
