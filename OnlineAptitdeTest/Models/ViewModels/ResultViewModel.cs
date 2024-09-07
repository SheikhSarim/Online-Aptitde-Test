using System.ComponentModel.DataAnnotations;

namespace OnlineAptitdeTest.Models.ViewModels
{
    public class ResultViewModel
    {
        [Key]
        public int ExampleId { get; set; }

        
        public string TestName { get; set; }

       
        public int TotalMarks { get; set; }

       
        public bool IsPassed { get; set; }
    }
}
