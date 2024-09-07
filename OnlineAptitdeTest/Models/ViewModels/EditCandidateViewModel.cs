using System.ComponentModel.DataAnnotations;
namespace OnlineAptitdeTest.Models.ViewModels
{

    public class EditCandidateViewModel
    {
        [Key]
        public int Id { get; set; }


        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
