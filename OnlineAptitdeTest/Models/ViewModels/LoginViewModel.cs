using System.ComponentModel.DataAnnotations;

namespace OnlineAptitdeTest.Models.ViewModels
{
    public class LoginViewModel
    {
        [Key]
        public int id{ get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

}
