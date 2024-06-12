using FinalExam.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalExam.Database.Interfaces
{
    public interface IPersonRepository
    {
        Task<Person> GetPersonByIdAsync(Guid personId);
        Task<List<Person>> GetAllPersonsAsync();
        Task AddPersonAsync(Person person);
        Task UpdatePersonAsync(Person person);
        Task DeletePersonAsync(Person person);
        Task<Person> GetPersonByUserIdAsync(Guid userId);
    }
}
