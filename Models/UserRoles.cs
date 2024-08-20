namespace CineMatrix_API.Models
{
    public class UserRoles
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public string Role { get; set; }


    }
}
