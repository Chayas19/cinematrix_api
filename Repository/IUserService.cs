using CineMatrix_API.Models;

namespace CineMatrix_API.Repository
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(User user);
    }
}