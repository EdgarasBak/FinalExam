using FinalExam.Database.Interfaces;
using FinalExam.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalExam.Database.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<User> GetUserAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Username == username);
        }
        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Id == userId);
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await SaveChangesAsync();
        }
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await SaveChangesAsync();
        }
        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await SaveChangesAsync();
        }
        private async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}