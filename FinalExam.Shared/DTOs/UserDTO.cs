

using FinalExam.Database.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FinalExam.Shared.DTOs
{
    public class UserDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]  //Do not output default value.
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.Regular;
    }
}
