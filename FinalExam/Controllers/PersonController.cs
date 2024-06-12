using FinalExam.BusinessLogic.Interfaces;
using FinalExam.Database.Interfaces;
using FinalExam.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FinalExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        private readonly IPersonRepository _personRepository;

        public PersonController(IPersonService personService, IPersonRepository personRepository)
        {
            _personService = personService;
            _personRepository = personRepository;
        }
        [HttpPost("CreatePerson")]
        [Authorize]
        public async Task<ActionResult<PersonDTO>> CreatePerson([FromForm] PersonDTO personDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return NotFound("User not found");
                }

                var createdPersonId = await _personService.CreatePersonAsync(personDto, Guid.Parse(userId));
                return Ok (createdPersonId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("GetPersonId")]
        [Authorize]
        public async Task<ActionResult<PersonDTO>> GetPersonById(Guid id)
        {
            var person = await _personService.GetPersonByIdAsync(id);
            if (person == null)
            {
                return NotFound("Person not found");
            }
            return Ok(person);
        }
        [HttpGet("GetAllPersons")]
        [Authorize]
        public async Task<ActionResult<List<PersonDTO>>> GetAllPersons()
        {
            try
            {
                var persons = await _personService.GetAllPersonsAsync();
                if (persons == null || !persons.Any())
                {
                    return NotFound("No persons found.");
                }
                return Ok(persons);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        [HttpPut("Update")]
        [Authorize]
        public async Task<ActionResult> UpdatePerson(Guid id, [FromForm] UpdatePersonDTO personDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return NotFound("User not found");
                }

                await _personService.UpdatePersonAsync(id, Guid.Parse(userId), personDto);
                var updatedPerson = await _personService.GetPersonByIdAsync(id);
                return Ok(updatedPerson);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        [Authorize]
        public async Task<ActionResult> DeletePerson(Guid id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return NotFound("User not found");
                }

                await _personService.DeletePersonAsync(id, Guid.Parse(userId));
                return Ok("Person deleted successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        [HttpGet("Photo")]
        [Authorize]
        public async Task<IActionResult> DownloadProfilePhoto(Guid id)
        {
            var person = await _personRepository.GetPersonByIdAsync(id);
            if (person == null || person.ProfilePhoto == null)
            {
                return NotFound("Profile photo not found");
            }
            return File(person.ProfilePhoto, "image/jpeg");          
        }

    }
}
