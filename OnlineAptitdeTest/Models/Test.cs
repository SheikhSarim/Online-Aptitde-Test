using OnlineAptitdeTest.Models.OnlineAptitdeTest.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineAptitdeTest.Models
{
    public class Test
    {
        [Key]
        public int TestId { get; set; } 
        public string TestName { get; set; }

        public bool isComplete { get; set; } = false;

        public ICollection<Question> Questions { get; set; }
        public ICollection<Result> Results { get; set; }
    }

}
