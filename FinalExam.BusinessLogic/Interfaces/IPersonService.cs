using FinalExam.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalExam.BusinessLogic.Interfaces
{
    public interface IPersonService
    {
        Task<PersonDTO> CreatePersonAsync(PersonDTO personDto, Guid userId);
        Task<PersonDTO> GetPersonByIdAsync(Guid personId);
        Task<List<PersonDTO>> GetAllPersonsAsync();
        Task UpdatePersonAsync(Guid personId, Guid userId, UpdatePersonDTO personDto);
        Task DeletePersonAsync(Guid personId, Guid userId);

    }
}
