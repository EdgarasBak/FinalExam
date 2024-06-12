using FinalExam.BusinessLogic.Interfaces;
using FinalExam.Database.Interfaces;
using FinalExam.Database.Models;
using FinalExam.Database.Repository;
using FinalExam.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace FinalExam.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public UserService(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<UserDTO> RegisterAsync(UserAuthDTO userDto)   
        {
            if (await _userRepository.GetUserAsync(userDto.Username) != null)
                throw new InvalidOperationException("Username already exists");

            if (userDto.Username.Length < 8 || userDto.Username.Length > 20)
                throw new ArgumentException("Username must be between 8 and 20 characters");

            if (IsPasswordComplex(userDto.Password))
                throw new ArgumentException("Password does not meet complexity requirements");

            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = userDto.Username,
                PasswordHash = passwordHash, 
                PasswordSalt = passwordSalt,
                Role = UserRole.Regular
            };
            await _userRepository.AddUserAsync(user);

            return new UserDTO { Username = user.Username, Role = user.Role };
        }
        private static bool IsPasswordComplex(string password)
        {
            if (password.Length < 12)
                return false;

            int upperCount = password.Count(char.IsUpper);
            int lowerCount = password.Count(char.IsLower);
            int digitCount = password.Count(char.IsDigit);
            int symbolCount = password.Count(c => char.IsSymbol(c) && !char.IsWhiteSpace(c));

            return upperCount >= 2 && lowerCount >= 2 && digitCount >= 2 && symbolCount >= 2;
        }
        public async Task<string> LoginAsync(UserAuthDTO userDto)
        {
            var user = await _userRepository.GetUserAsync(userDto.Username);
            if (user == null || !VerifyPasswordHash(userDto.Password, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException("Invalid username or password");
            string role = user.Role.ToString();
            var token = _jwtService.GenerateJwtToken(user.Username, role, user.Id);
            return token;
        }

        public async Task UpdateUserAsync(Guid userId, UserDTO userDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null || user.Id != userId)
                throw new UnauthorizedAccessException("You can only update your own information");

            if (!string.IsNullOrWhiteSpace(userDto.Password))
            {
                CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            user.Username = userDto.Username ?? user.Username;

            await _userRepository.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(Guid userId, string adminUsername)
        {
            var adminUser = await _userRepository.GetUserAsync(adminUsername);
            if (adminUser == null || adminUser.Role != UserRole.Admin)
                throw new UnauthorizedAccessException("Only admins can delete user accounts");

            if (adminUser.Id == userId)
                throw new InvalidOperationException("Admins cannot delete their own account");

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            await _userRepository.DeleteUserAsync(user);
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(u => new UserDTO { Username = u.Username, Role = u.Role }).ToList();
        }

        public async Task<UserDTO> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            return new UserDTO { Username = user.Username, Role = user.Role };
        }

        private static  void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
}
