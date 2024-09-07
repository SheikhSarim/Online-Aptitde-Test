using System.ComponentModel.DataAnnotations;

namespace OnlineAptitdeTest.Models
{
    namespace OnlineAptitdeTest.Models
    {
        public class Question
        {
            [Key]
            public int QuestionId { get; set; } 
            public string QuestionText { get; set; }

            public string OptionA { get; set; }
            public string OptionB { get; set; }
            public string OptionC { get; set; }
            public string OptionD { get; set; }
            public string CorrectOption { get; set; }

            public int TestId { get; set; } 

            public Test Test { get; set; }
        }
    }

}
