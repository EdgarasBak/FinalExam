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
    public class PersonRepository : IPersonRepository
    {
        private readonly AppDbContext _context;

        public PersonRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Person> GetPersonByIdAsync(Guid personId)
        {
            return await _context.Persons.Include(p => p.PlaceOfResidence).SingleOrDefaultAsync(p => p.Id == personId);
        }
        public async Task<List<Person>> GetAllPersonsAsync() 
        {
            return await _context.Persons.Include(p => p.PlaceOfResidence).ToListAsync();
        }
        public async Task AddPersonAsync(Person person)
        {
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();
        }
        public async Task UpdatePersonAsync(Person person)
        {
            _context.Persons.Update(person);
            await _context.SaveChangesAsync();
        }
        public async Task DeletePersonAsync(Person person)
        {
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
        }
        public async Task<Person> GetPersonByUserIdAsync(Guid userId)
        {
            return await _context.Persons.Include(p => p.PlaceOfResidence).SingleOrDefaultAsync(p => p.UserId == userId);
        }
    }
}
