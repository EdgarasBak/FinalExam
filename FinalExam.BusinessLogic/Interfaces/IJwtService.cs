

namespace FinalExam.BusinessLogic.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(string username, string role, Guid userId);
    }
}
