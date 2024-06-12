using FinalExam.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalExam.Database.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(string username);
        Task<User> GetUserByIdAsync(Guid userId);
        Task<List<User>> GetAllUsersAsync();
        Task AddUserAsync(User user); 
        Task UpdateUserAsync(User user); 
        Task DeleteUserAsync(User user);
    }
}
