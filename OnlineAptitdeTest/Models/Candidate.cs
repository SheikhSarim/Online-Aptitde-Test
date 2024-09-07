namespace OnlineAptitdeTest.Models
{
    public class Candidate
    {
        public int CandidateId { get; set; } 
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }


        public ICollection<Result> Results { get; set; }

        
    }
}
