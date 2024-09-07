using System.ComponentModel.DataAnnotations;

namespace OnlineAptitdeTest.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public List<UserRole> UserRoles { get; set; }
    }
}
