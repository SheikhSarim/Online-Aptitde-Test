using System.ComponentModel.DataAnnotations;

namespace OnlineAptitdeTest.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public List<UserRole> UserRoles { get; set; }
    }

}
