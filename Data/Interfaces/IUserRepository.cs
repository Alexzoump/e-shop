using System.Threading.Tasks;
using Data.Models;

namespace Data.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsAsync(string username, string email);
        Task AddAsync(User user);
        Task<User> GetByUsernameAsync(string username);
    }
} 