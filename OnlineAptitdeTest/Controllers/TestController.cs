using Microsoft.AspNetCore.Mvc;
using OnlineAptitdeTest.Data;
using OnlineAptitdeTest.Models.OnlineAptitdeTest.Models;
using OnlineAptitdeTest.Models;
using OnlineAptitdeTest.Models.ViewModels;
using Microsoft.EntityFrameworkCore;



namespace OnlineAptitdeTest.Controllers
{
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TestController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            ViewBag.ShowTimer = false;
            var tests = _context.Tests
                         .Select(t => new TestViewModel
                         {
                             TestId = t.TestId,
                             TestName = t.TestName,
                             Questions = t.Questions.Select(q => new QuestionViewModel
                             {
                                 QuestionId = q.QuestionId,
                                 QuestionText = q.QuestionText,
                                 OptionA = q.OptionA,
                                 OptionB = q.OptionB,
                                 OptionC = q.OptionC,
                                 OptionD = q.OptionD
                             }).ToList()
                         }).ToList();

            return View(tests);

        }



        // Method to fetch questions for a specific test
        public IActionResult AptitudeTest(int testId )
        {
            ViewBag.ShowTimer = true;

            var candidateIdString = HttpContext.Session.GetString("CandidateId");
            if (string.IsNullOrEmpty(candidateIdString) || !int.TryParse(candidateIdString, out int candidateId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if no candidate is found
            }

            bool hasSubmitted = _context.Results.Any(r => r.TestId == testId && r.CandidateId == candidateId);

            if (hasSubmitted)
            {
                return RedirectToAction("TestAlreadySubmitted");
            }


            var test = _context.Tests
                               .Where(t => t.TestId == testId)
                               .Select(t => new
                               {
                                   t.TestId,
                                   t.TestName,
                                   Questions = t.Questions.Select(q => new
                                   {
                                       q.QuestionId,
                                       q.QuestionText,
                                       q.OptionA,
                                       q.OptionB,
                                       q.OptionC,
                                       q.OptionD
                                   }).ToList()
                               }).FirstOrDefault();

            if (test == null)
            {
                return NotFound();
            }

            ViewBag.TestId = testId;

            return View(test); 
        }

        [HttpPost]
        public IActionResult UserSelectAnswers(int testId, UserAnswersViewModel userAnswers)
        {

            var test = _context.Tests.Find(testId);

            if (test == null)
            {
                Console.WriteLine($"Test not found for testId: {testId}");
                return NotFound();
            }

            var questions = _context.Questions
                                    .Where(q => q.TestId == testId)
                                    .ToList();

            var answeredCorrectly = new HashSet<int>();
            int correctAnswersCount = 0;

            foreach (var question in questions)
            {
                if (userAnswers.Answers.TryGetValue(question.QuestionId, out var userAnswer))
                {
                    if (userAnswer.Equals(question.CorrectOption, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!answeredCorrectly.Contains(question.QuestionId))
                        {
                            correctAnswersCount += 5;
                            answeredCorrectly.Add(question.QuestionId); 
                            Console.WriteLine($"Correct Answer for Question ID: {question.QuestionId}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Incorrect Answer for Question ID: {question.QuestionId}. Selected: {userAnswer}, Correct: {question.CorrectOption}");
                    }
                }
            }
            var candidateIdString = _httpContextAccessor.HttpContext.Session.GetString("CandidateId");
            int candidateId = int.Parse(candidateIdString ?? "0"); 

            var result = new Result
            {
                CandidateId = candidateId,
                TestId = testId,
                TotalMarks = correctAnswersCount,
                IsPassed = correctAnswersCount >= 15 
            };

            test.isComplete = true;
            _context.Results.Add(result);
            _context.SaveChanges();



            int nextTestId;
            string nextTestName;

            switch (test.TestName)
            {
                case "General Knowledge":
                    nextTestName = "Mathematics";
                    break;
                case "Mathematics":
                    nextTestName = "Computer Technology";
                    break;
                case "Computer Technology":
                    if (AreAllTestsComplete())
                    {   
                        MarkAllTestsAsIncomplete();
                        return RedirectToAction("Result");
                    }
                    return View("Error");
                default:
                    return View("Error");
            }

            nextTestId = GetNextTestId(nextTestName);

            if (nextTestId == 0) 
            {
                return RedirectToAction("Index"); 
            }

            return RedirectToAction("AptitudeTest", new { testId = nextTestId });
        }
        public IActionResult TestAlreadySubmitted()
        {
            return View(); 
        }

        private void MarkAllTestsAsIncomplete()
        {
            var allTests = _context.Tests.ToList();
            foreach (var test in allTests)
            {
                test.isComplete = false;
            }
            _context.SaveChanges();
        }



        private bool AreAllTestsComplete()
        {
            var allTests = _context.Tests.ToList();
            return allTests.All(t => t.isComplete);
        }



        private int GetNextTestId(string testName)
        {
            return _context.Tests
                  .Where(t => t.TestName == testName)
                  .Select(t => t.TestId)
                  .FirstOrDefault();
        }


        public IActionResult Result()
        {
            var candidateIdString = _httpContextAccessor.HttpContext.Session.GetString("CandidateId");
            int candidateId = int.Parse(candidateIdString ?? "0");

            var results = _context.Results
                .Where(r => r.CandidateId == candidateId)
                .Select(r => new ResultViewModel
                {
                    TestName = r.Test.TestName,
                    TotalMarks = r.TotalMarks,
                    IsPassed = r.IsPassed
                }).ToList();

            int totalScore = results.Sum(r => r.TotalMarks);


            double percentage = CalculatePercentage(totalScore);


            percentage = Math.Round(percentage, 2);

            ViewBag.TotalScore = totalScore;
            ViewBag.Percentage = percentage;

            return View(results);
        }

        public double CalculatePercentage(double totalScore)
        {
            int questionCount = _context.Questions.Count();

            double maxScore = questionCount * 5.0;  

            double percentage = (totalScore / maxScore) * 100;

            return percentage;
        }
    }
}

