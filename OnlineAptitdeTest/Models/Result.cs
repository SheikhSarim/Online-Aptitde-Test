using static System.Net.Mime.MediaTypeNames;

namespace OnlineAptitdeTest.Models
{
    public class Result
    {
        public int ResultId { get; set; }
        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; }

        public int TestId { get; set; }
        public Test Test { get; set; }

        public int TotalMarks { get; set; }
        public bool IsPassed { get; set; }
      
    }

}
