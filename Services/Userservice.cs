using CineMatrix_API.Models;
using CineMatrix_API.Repository;
using Microsoft.EntityFrameworkCore;

namespace CineMatrix_API.Services
{

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Roles) 
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (existingUser != null)
            {
            
                existingUser.Email = user.Email;

             
                _context.UserRoles.RemoveRange(existingUser.Roles);
                if (user.Roles != null)
                {
                    _context.UserRoles.AddRange(user.Roles); 
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}


