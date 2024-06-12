

namespace FinalExam.Database.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public UserRole Role { get; set; } = UserRole.Regular;
        public ICollection<Person> Persons { get; set; } // One-to-Many relationship with Person
    }
}
