using System.ComponentModel.DataAnnotations;

namespace OnlineAptitdeTest.Models.ViewModels
{
    public class TestViewModel
    {
        [Key]
        public int TestId { get; set; }
        public string TestName { get; set; }
        public List<QuestionViewModel> Questions { get; set; }

        // Property to hold the number of questions
        public int TotalQuestions => Questions?.Count ?? 0;
    }

}
