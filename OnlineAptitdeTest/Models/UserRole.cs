using System.Data;

namespace OnlineAptitdeTest.Models
{
    public class UserRole
    {
        public int UserId { get; set; }

        public Admin Admins { get; set; }


        public Manager Managers { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
