using System.ComponentModel.DataAnnotations;

namespace OnlineAptitdeTest.Models
{
    public class Manager
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } 
        public List<UserRole> UserRoles { get; set; }
    }
}
