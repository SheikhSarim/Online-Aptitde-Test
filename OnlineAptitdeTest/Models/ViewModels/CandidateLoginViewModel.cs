using System.ComponentModel.DataAnnotations;

namespace OnlineAptitdeTest.Models.ViewModels
{
    public class CandidateLoginViewModel
    {
        [Key]
        public int id { get; set; }

        [Required]
        
        public string Username { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
