using CineMatrix_API.DTOs;
using CineMatrix_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Summary = "Assigns a role to a user.",
                          Description = "Assigns the specified role to the user with the provided user ID. Returns an error if the role is already assigned or if the user already has roles.")]

        public async Task<IActionResult> AssignRole([FromBody] UserRolesDTO userRolesDto)
        {
            if (userRolesDto == null || string.IsNullOrWhiteSpace(userRolesDto.RoleName) || userRolesDto.RoleName == "string" || userRolesDto.UserId <= 0)
            {
                return BadRequest("Invalid role assignment data.");
            }

            var user = await _context.Users.FindAsync(userRolesDto.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roleAlreadyAssigned = await _context.UserRoles.AnyAsync(ur => ur.UserId == userRolesDto.UserId && ur.Role == userRolesDto.RoleName);

            if (roleAlreadyAssigned)
            {
                return BadRequest("Role already assigned to this user.");
            }

            var existingRoles = await _context.UserRoles.Where(ur => ur.UserId == userRolesDto.UserId).Select(ur => ur.Role).ToListAsync();

            if (existingRoles.Any())
            {
                var existingRolesList = string.Join(", ", existingRoles);
                return BadRequest($"User already has the following roles assigned: {existingRolesList}");
            }

            var userRole = new UserRoles
            {
                UserId = userRolesDto.UserId,
                Role = userRolesDto.RoleName
            };

            try
            {

                await _context.UserRoles.AddAsync(userRole);
                await _context.SaveChangesAsync();

                return Ok("Role assigned successfully.");
            }
            catch (DbUpdateException ex)
            {

                return StatusCode(500, "An error occurred while assigning the role. Please try again later.");

            }

            catch (Exception ex)
            {

                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }

        }

        [HttpPut("update-role")]
        [SwaggerOperation(Summary = "Updates the role of a user.",
                          Description = "Updates the role of the user with the provided user ID. Returns an error if the user role is not found or if the role is already set to the specified value.")]
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
        [SwaggerOperation(Summary = "Deletes roles by user ID.",
                          Description = "Deletes all roles associated with the user ID. Returns an error if no roles are found for the specified user ID.")]
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


        [SwaggerOperation(Summary = "Gets roles by user ID.",
                     Description = "Retrieves a list of roles assigned to the user with the specified rolename. Returns an error if the user is not found.")]
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
        [SwaggerOperation(Summary = "Gets roles by user ID.",
                          Description = "Retrieves a list of roles assigned to the user with the specified user ID. Returns an error if the user is not found.")]
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
