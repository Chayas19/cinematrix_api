using CineMatrix_API.DTOs;
using CineMatrix_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineMatrix_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserRolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Assign a role to a user
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRolesDTO userRolesDto)
        {
            if (userRolesDto == null || string.IsNullOrWhiteSpace(userRolesDto.RoleName))
            {
                return BadRequest("Invalid role assignment data.");
            }

            var user = await _context.Users.FindAsync(userRolesDto.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var existingRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userRolesDto.UserId && ur.Role == userRolesDto.RoleName);

            if (existingRole != null)
            {
                return BadRequest("Role already assigned to this user.");
            }

            var userRole = new UserRoles
            {
                UserId = userRolesDto.UserId,
                Role = userRolesDto.RoleName 
            };

            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();

            return Ok("Role assigned successfully.");
        }

        [HttpPut("update-role")]
        public async Task<IActionResult> UpdateRole([FromBody] UserRolesDTO userRolesDto)
        {
            if (userRolesDto == null || string.IsNullOrWhiteSpace(userRolesDto.RoleName))
            {
                return BadRequest("Invalid role update data.");
            }

            var roleName = userRolesDto.RoleName.Trim();

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userRolesDto.UserId);

            if (userRole == null)
            {
                return NotFound("User role not found.");
            }

            if (!string.Equals(userRole.Role, roleName, StringComparison.OrdinalIgnoreCase))
            {
                userRole.Role = roleName;
                _context.UserRoles.Update(userRole);
                await _context.SaveChangesAsync();

                return Ok("Role updated successfully.");
            }

            return BadRequest("Role is already set to the specified value.");
        }

        [HttpDelete("delete-role-by-userid")]
        public async Task<IActionResult> DeleteRoleByUserId([FromBody] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            if (userRoles.Count == 0)
            {
                return NotFound("No roles found for the specified user ID.");
            }

            _context.UserRoles.RemoveRange(userRoles);
            await _context.SaveChangesAsync();

            return Ok("Roles removed successfully for the specified user ID.");
        }
        [HttpDelete("delete-role-by-rolename")]
        public async Task<IActionResult> DeleteRoleByRoleName([FromBody] UserRolesDTO userRolesDto)
        {
            if (userRolesDto == null || string.IsNullOrWhiteSpace(userRolesDto.RoleName))
            {
                return BadRequest("Invalid role deletion data.");
            }

            var userRoles = await _context.UserRoles
                .Where(ur => ur.Role == userRolesDto.RoleName)
                .ToListAsync();

            if (userRoles.Count == 0)
            {
                return NotFound("No roles found with the specified role name.");
            }

            _context.UserRoles.RemoveRange(userRoles);
            await _context.SaveChangesAsync();

            return Ok("Roles removed successfully for the specified role name.");
        }

        // Get user roles by user ID
        [HttpGet("get-roles/{userId}")]
        public async Task<IActionResult> GetRoles(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role)
                .ToListAsync();

            return Ok(roles);
        }
    }
}
