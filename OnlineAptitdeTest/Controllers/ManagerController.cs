using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineAptitdeTest.Models.ViewModels;
using OnlineAptitdeTest.Models;
using OnlineAptitdeTest.Data;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineAptitdeTest.Models.OnlineAptitdeTest.Models;
using System.Diagnostics;

namespace OnlineAptitdeTest.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ManagerController> _logger;

        public ManagerController(ApplicationDbContext context, ILogger<ManagerController> logger)
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
        public async Task<IActionResult> Login(LoginViewModel model)
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

                    _logger.LogInformation("Manager logged in successfully.");

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

            _logger.LogInformation("Manager logged out successfully.");

            return RedirectToAction("Login");
        }

        private Manager AuthenticateUser(string email, string password)
        {
            var userFromDb = _context.Managers
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
        public IActionResult CreateCandidate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCandidate(CreateCandidateViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "The password and confirmation password do not match.");
                return View(model);
            }

            var existingCandidate = await _context.Candidates.FirstOrDefaultAsync(c => c.Email == model.Email);
            if (existingCandidate != null)
            {
                ModelState.AddModelError(nameof(model.Email), "The email address is already in use.");
                return View(model);
            }


            if (ModelState.IsValid)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var candidate = new Candidate
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = hashedPassword
                };

                _context.Candidates.Add(candidate);
                await _context.SaveChangesAsync();
                return RedirectToAction("CandidateList");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditCandidate(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }

            var model = new EditCandidateViewModel
            {
                Id = candidate.CandidateId,
                Username = candidate.Username,
                Email = candidate.Email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditCandidate(EditCandidateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var candidate = await _context.Candidates.FindAsync(model.Id);
                if (candidate == null)
                {
                    return NotFound();
                }

                candidate.Username = model.Username;
                candidate.Email = model.Email;

                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    if (model.NewPassword != model.ConfirmNewPassword)
                    {
                        ModelState.AddModelError(string.Empty, "The new password and confirmation password do not match.");
                        return View(model);
                    }

                    candidate.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                }

                _context.Candidates.Update(candidate);
                await _context.SaveChangesAsync();
                return RedirectToAction("CandidateList");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCandidate(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }
            return View(candidate);
        }

        [HttpPost, ActionName("DeleteCandidate")]
        public async Task<IActionResult> DeleteCandidateConfirmed(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate != null)
            {
                _context.Candidates.Remove(candidate);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("CandidateList");
        }


        public IActionResult ViewCandidateResults()
        {
            var candidates = _context.Candidates
                                     .Include(c => c.Results)
                                     .ThenInclude(r => r.Test)
                                     .ToList();

            if (candidates == null || !candidates.Any())
            {
                return NotFound("No candidates found.");
            }

            var viewModel = candidates.Select(candidate =>
            {
                int totalScore = candidate.Results.Sum(r => r.TotalMarks);
                double percentage = CalculatePercentage(totalScore, _context.Questions.Count());

                return new CandidateResultViewModel
                {
                    Candidate = candidate,
                    Results = candidate.Results.Select(r => new ResultViewModel
                    {
                        TestName = r.Test.TestName,
                        TotalMarks = r.TotalMarks,
                        IsPassed = r.IsPassed
                    }).ToList(),
                    TotalScore = totalScore,
                    Percentage = percentage
                };
            }).ToList();

            return View(viewModel);
        }

        // Updated CalculatePercentage method to accept the number of results
        public double CalculatePercentage(double totalScore, int questionCount)
        {

            double maxScore = questionCount * 5.0;

            double percentage = (totalScore / maxScore) * 100;

            return percentage;
        }


        [HttpGet]
        public async Task<IActionResult> CandidateList()
        {
            var candidates = await _context.Candidates.ToListAsync();
            return View(candidates);
        }
        // GET: Test/CreateTest
        public IActionResult CreateTest()
        {
            return View();
        }

        public IActionResult Transfer()
        {
            return View();
        }

        // POST: Test/CreateTest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTest(Test test)
        {

            _context.Tests.Add(test);
            await _context.SaveChangesAsync();
            return RedirectToAction("IndexTest");


        }

        // GET: Test/EditTest/5
        public async Task<IActionResult> EditTest(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests.FindAsync(id);
            if (test == null)
            {
                return NotFound();
            }
            return View(test);
        }

        // POST: Test/EditTest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTest(int id, [Bind("TestId,TestName,isComplete")] Test test)
        {
            if (id == test.TestId)
            {
                try
                {
                    _context.Update(test);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestExists(test.TestId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(IndexTest));
            }
            return View(test);
        }

        // GET: Test/DeleteTest/5
        public async Task<IActionResult> DeleteTest(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests
                .FirstOrDefaultAsync(m => m.TestId == id);
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }

        // POST: Test/DeleteTest/5
        [HttpPost, ActionName("DeleteTest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTestConfirmed(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexTest));
        }

        // GET: Test/IndexTest
        public async Task<IActionResult> IndexTest()
        {
            var testViewModels = await _context.Tests
                .Select(test => new TestViewModel
                {
                    TestId = test.TestId,
                    TestName = test.TestName,
                    Questions = test.Questions.Select(q => new QuestionViewModel
                    {
                        QuestionId = q.QuestionId,
                        QuestionText = q.QuestionText
                    }).ToList()
                })
                .ToListAsync();

            return View(testViewModels);
        }

        // GET: Test/DetailsTest/5
        public async Task<IActionResult> DetailsTest(int id)
        {
            var test = await _context.Tests
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.TestId == id);

            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }

        private bool TestExists(int id)
        {
            return _context.Tests.Any(e => e.TestId == id);
        }
        // GET: Question
        public async Task<IActionResult> GetQuestions()
        {
            var questions = await _context.Questions.Include(q => q.Test).ToListAsync();
            return View(questions);
        }

        // GET: Question/Details/5
        public async Task<IActionResult> GetQuestionDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.Test)
                .FirstOrDefaultAsync(m => m.QuestionId == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // GET: Question/Create
        public IActionResult CreateQuestion()
        {
            ViewData["TestId"] = new SelectList(_context.Tests, "TestId", "TestName");
            return View();
        }

        // POST: Question/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestion([Bind("QuestionId,QuestionText,OptionA,OptionB,OptionC,OptionD,CorrectOption,TestId")] Question question)
        {
            try
            {
                _context.Add(question);
                int changes = await _context.SaveChangesAsync();

                if (changes > 0)
                {
                    return RedirectToAction(nameof(GetQuestions));
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while saving the question.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
            }
            ViewData["TestId"] = new SelectList(_context.Tests, "TestId", "TestName", question.TestId);
            return View(question);
        }

        // GET: Question/Edit/5
        public async Task<IActionResult> EditQuestion(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            ViewData["TestId"] = new SelectList(_context.Tests, "TestId", "TestName", question.TestId);
            return View(question);
        }

        // POST: Question/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestion(int id, [Bind("QuestionId,QuestionText,OptionA,OptionB,OptionC,OptionD,CorrectOption,TestId")] Question question)
        {
            if (id != question.QuestionId)
            {
                return NotFound();
            }

            var existingQuestion = await _context.Questions.FindAsync(id);
            if (existingQuestion == null)
            {
                return NotFound();
            }

            try
            {
                _context.Entry(existingQuestion).CurrentValues.SetValues(question);
                int changes = await _context.SaveChangesAsync();

                if (changes > 0)
                {
                    return RedirectToAction(nameof(GetQuestions));
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while updating the question.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(question.QuestionId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
            }

            ViewData["TestId"] = new SelectList(_context.Tests, "TestId", "TestName", question.TestId);
            return View(question);
        }



        // GET: Question/Delete/5
        public async Task<IActionResult> DeleteQuestion(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.Test)
                .FirstOrDefaultAsync(m => m.QuestionId == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Question/Delete/5
        [HttpPost, ActionName("DeleteQuestion")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestionConfirmed(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetQuestions));
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.QuestionId == id);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
