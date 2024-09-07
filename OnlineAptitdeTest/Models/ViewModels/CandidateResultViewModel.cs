using System.ComponentModel.DataAnnotations;

namespace OnlineAptitdeTest.Models.ViewModels
{
    public class CandidateResultViewModel
    {
        [Key]
        public int id { get; set; }
       
        public Candidate Candidate { get; set; } 

        public List<ResultViewModel> Results { get; set; }


        public int TotalScore { get; set; } 
        public double Percentage { get; set; } 

    }
}
