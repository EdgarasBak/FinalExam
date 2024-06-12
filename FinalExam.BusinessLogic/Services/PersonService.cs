using FinalExam.BusinessLogic.Interfaces;
using FinalExam.Database.Interfaces;
using FinalExam.Database.Models;
using FinalExam.Shared.DTOs;
using FinalExam.Shared.Helpers;
using FluentValidation;


namespace FinalExam.BusinessLogic.Services
{
    public class PersonService : IPersonService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPersonRepository _personRepository;

        public PersonService(IUserRepository userRepository, IPersonRepository personRepository)
        {
            _userRepository = userRepository;
            _personRepository = personRepository;
        }
        public PersonService(IPersonRepository personRepository, IUserRepository userRepository)
        {
            _personRepository = personRepository;
            _userRepository = userRepository;
        }

        public async Task<PersonDTO> CreatePersonAsync(PersonDTO personDto, Guid userId)
        {
            var validationResult = new PersonInformationValidator().Validate(personDto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var person = new Person
            {
                Name = personDto.Name,
                LastName = personDto.LastName,
                Gender = personDto.Gender,
                Birthday = DateOnly.FromDateTime(personDto.Birthday),
                PersonalCode = personDto.PersonalCode,
                TelephoneNumber = personDto.TelephoneNumber,
                Email = personDto.Email,
                ProfilePhoto = personDto.ProfilePhoto != null ? await ImageHelper.SaveAndResizeProfilePhotoAsync(personDto.ProfilePhoto) : null, 
                UserId = userId,
                PlaceOfResidence = new PlaceOfResidence
                {
                    City = personDto.PlaceOfResidence.City,
                    Street = personDto.PlaceOfResidence.Street,
                    HouseNumber = personDto.PlaceOfResidence.HouseNumber,
                    ApartmentNumber = personDto.PlaceOfResidence.ApartmentNumber
                }
            };

            await _personRepository.AddPersonAsync(person);

            return personDto;
        }

        public async Task<PersonDTO> GetPersonByIdAsync(Guid personId)
        {
            var person = await _personRepository.GetPersonByIdAsync(personId);
            if (person == null)
                throw new KeyNotFoundException("Person not found");

            var personDto = new PersonDTO
            {
                Name = person.Name,
                LastName = person.LastName,
                Gender = person.Gender,
                Birthday = person.Birthday.ToDateTime(TimeOnly.MinValue),
                PersonalCode = person.PersonalCode,
                TelephoneNumber = person.TelephoneNumber,
                Email = person.Email,
                PlaceOfResidence = new PlaceOfResidenceDTO
                {
                    City = person.PlaceOfResidence.City,
                    Street = person.PlaceOfResidence.Street,
                    HouseNumber = person.PlaceOfResidence.HouseNumber,
                    ApartmentNumber = person.PlaceOfResidence.ApartmentNumber
                }
            };

            return personDto;
        }

        public async Task<List<PersonDTO>> GetAllPersonsAsync()
        {
            var persons = await _personRepository.GetAllPersonsAsync();
            return persons.Select(p => new PersonDTO
            {
                Name = p.Name,
                LastName = p.LastName,
                Gender = p.Gender,
                Birthday = p.Birthday.ToDateTime(TimeOnly.MinValue),
                PersonalCode = p.PersonalCode,
                TelephoneNumber = p.TelephoneNumber,
                Email = p.Email,
                PlaceOfResidence = new PlaceOfResidenceDTO
                {
                    City = p.PlaceOfResidence.City,
                    Street = p.PlaceOfResidence.Street,
                    HouseNumber = p.PlaceOfResidence.HouseNumber,
                    ApartmentNumber = p.PlaceOfResidence.ApartmentNumber
                }
            }).ToList();
        }
        public async Task UpdatePersonAsync(Guid personId, Guid userId, UpdatePersonDTO personDto)
        {
            var person = await _personRepository.GetPersonByIdAsync(personId);
            if (person == null)
                throw new KeyNotFoundException("Person not found");

            if (person.UserId != userId)
                throw new UnauthorizedAccessException("You can only update your own information");

            var validationResult = new UpdatePersonValidator().Validate(personDto);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
                var errorMessageString = string.Join("; ", errorMessages);
                throw new ValidationException(errorMessageString);
            }

            if (!string.IsNullOrWhiteSpace(personDto.Name)) person.Name = personDto.Name;
            if (!string.IsNullOrWhiteSpace(personDto.LastName)) person.LastName = personDto.LastName;
            if (!string.IsNullOrWhiteSpace(personDto.Gender)) person.Gender = personDto.Gender;
            if (personDto.Birthday.HasValue) person.Birthday = DateOnly.FromDateTime(personDto.Birthday.Value);
            if (!string.IsNullOrWhiteSpace(personDto.PersonalCode)) person.PersonalCode = personDto.PersonalCode;
            if (!string.IsNullOrWhiteSpace(personDto.TelephoneNumber)) person.TelephoneNumber = personDto.TelephoneNumber;
            if (!string.IsNullOrWhiteSpace(personDto.Email)) person.Email = personDto.Email;

            if (personDto.ProfilePhoto != null)
            {
                person.ProfilePhoto = await ImageHelper.SaveAndResizeProfilePhotoAsync(personDto.ProfilePhoto);
            }

            if (personDto.PlaceOfResidence != null)
            {
                var placeOfResidenceValidationResult = new UpdatePlaceOfResidenceValidator().Validate(personDto.PlaceOfResidence);
                if (!placeOfResidenceValidationResult.IsValid)
                {
                    var errorMessages = placeOfResidenceValidationResult.Errors.Select(e => e.ErrorMessage);
                    var errorMessageString = string.Join("; ", errorMessages);
                    throw new ValidationException(errorMessageString);
                }

                if (person.PlaceOfResidence == null)
                {
                    person.PlaceOfResidence = new PlaceOfResidence();
                }

                if (!string.IsNullOrWhiteSpace(personDto.PlaceOfResidence.City)) person.PlaceOfResidence.City = personDto.PlaceOfResidence.City;
                if (!string.IsNullOrWhiteSpace(personDto.PlaceOfResidence.Street)) person.PlaceOfResidence.Street = personDto.PlaceOfResidence.Street;
                if (!string.IsNullOrWhiteSpace(personDto.PlaceOfResidence.HouseNumber)) person.PlaceOfResidence.HouseNumber = personDto.PlaceOfResidence.HouseNumber;
                if (!string.IsNullOrWhiteSpace(personDto.PlaceOfResidence.ApartmentNumber)) person.PlaceOfResidence.ApartmentNumber = personDto.PlaceOfResidence.ApartmentNumber;
            }

            await _personRepository.UpdatePersonAsync(person);
        }
        public async Task DeletePersonAsync(Guid personId, Guid userId)
        {
            var person = await _personRepository.GetPersonByIdAsync(personId);
            if (person == null)
                throw new KeyNotFoundException("Person not found");

            if (person.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own information");

            await _personRepository.DeletePersonAsync(person);
        }
    }
}

