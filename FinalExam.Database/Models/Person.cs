

namespace FinalExam.Database.Models
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateOnly Birthday { get; set; }
        public string PersonalCode { get; set; }
        public string TelephoneNumber { get; set; }
        public string Email { get; set; }
        public byte[] ProfilePhoto { get; set; } 
        public PlaceOfResidence PlaceOfResidence { get; set; } // One-to-one relationship
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
