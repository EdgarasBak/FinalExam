using FinalExam.Shared.DTOs;


namespace FinalExam.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> RegisterAsync(UserAuthDTO userDto);
        Task<string> LoginAsync(UserAuthDTO userDto);
        Task UpdateUserAsync(Guid userId, UserDTO userDto);
        Task DeleteUserAsync(Guid userId, string adminUsername);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(Guid userId);
    }
}
