namespace OnlineAptitdeTest.Models.ViewModels
{
    public class UserAnswersViewModel
    {
        public int TestId { get; set; }
        public Dictionary<int, string> Answers { get; set; } = new Dictionary<int, string>();
    }
}
