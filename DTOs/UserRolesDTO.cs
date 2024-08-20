using System.ComponentModel.DataAnnotations;
using CineMatrix_API.Enums;

namespace CineMatrix_API.DTOs
{
    public class UserRolesDTO

    {

        public int UserId { get; set; }

        [Required]
        public string RoleName { get; set; }

     
        public RoleType GetRoleType()
        {
            if (Enum.TryParse<RoleType>(RoleName, true, out var role))
            {
                return role;
            }
            throw new ArgumentException("Invalid role name.");
        }
    }

}

