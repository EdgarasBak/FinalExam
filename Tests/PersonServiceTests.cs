using FinalExam.BusinessLogic.Services;
using FinalExam.Database.Interfaces;
using FinalExam.Database.Models;
using FinalExam.Shared.DTOs;
using FluentValidation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class PersonServiceTests
    {
        [Fact]
        public async Task CreatePersonAsync_ReturnsPersonDto_WhenSuccessful()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var personRepositoryMock = new Mock<IPersonRepository>();

            var personDto = new PersonDTO
            {
                Name = "Johny",
                LastName = "Johnas",
                Gender = "Male",
                Birthday = new DateTime(1915 - 05 - 23),
                PersonalCode = "12345678901",
                TelephoneNumber = "+37012345678",
                Email = "johnjohnas@gmail.com",
                PlaceOfResidence = new PlaceOfResidenceDTO
                {
                    City = "NewYork",
                    Street = "Main Street",
                    HouseNumber = "123",
                    ApartmentNumber = "456"
                }
            };

            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Username = "john_doe" }; 
            userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            var service = new PersonService(userRepositoryMock.Object, personRepositoryMock.Object);

            // Act
            var result = await service.CreatePersonAsync(personDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(personDto.Name, result.Name);
            Assert.Equal(personDto.LastName, result.LastName);
            Assert.Equal(personDto.Gender, result.Gender);
            Assert.Equal(personDto.Birthday, result.Birthday);
            Assert.Equal(personDto.PersonalCode, result.PersonalCode);
            Assert.Equal(personDto.TelephoneNumber, result.TelephoneNumber);
            Assert.Equal(personDto.Email, result.Email);
            Assert.Equal(personDto.PlaceOfResidence.City, result.PlaceOfResidence.City);
            Assert.Equal(personDto.PlaceOfResidence.Street, result.PlaceOfResidence.Street);
            Assert.Equal(personDto.PlaceOfResidence.HouseNumber, result.PlaceOfResidence.HouseNumber);
            Assert.Equal(personDto.PlaceOfResidence.ApartmentNumber, result.PlaceOfResidence.ApartmentNumber);
        }
        [Fact]
        public async Task CreatePersonAsync_ThrowsKeyNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var personRepositoryMock = new Mock<IPersonRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null);

            var service = new PersonService(userRepositoryMock.Object, personRepositoryMock.Object);
            var personDto = new PersonDTO();

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(async () => await service.CreatePersonAsync(personDto, Guid.NewGuid()));
        }
        [Fact]
        public async Task GetPersonByIdAsync_ReturnsPersonDto_WhenPersonExists()
        {
            // Arrange
            var personRepositoryMock = new Mock<IPersonRepository>();
            var personId = Guid.NewGuid();
            var person = new Person 
            {
                Id = personId,
                Name = "John",
                LastName = "Johnas",
                Gender = "Male",
                Birthday = new DateOnly(1990, 1, 1),
                PersonalCode = "1234567890",
                TelephoneNumber = "123456789",
                Email = "johnjohnas@gmail.com",
                PlaceOfResidence = new PlaceOfResidence
                {
                    City = "New York",
                    Street = "Main Street",
                    HouseNumber = "123",
                    ApartmentNumber = "456"
                }
            };
            personRepositoryMock.Setup(repo => repo.GetPersonByIdAsync(personId)).ReturnsAsync(person);

            var service = new PersonService(null, personRepositoryMock.Object);

            // Act
            var result = await service.GetPersonByIdAsync(personId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetPersonByIdAsync_ThrowsKeyNotFoundException_WhenPersonNotFound()
        {
            // Arrange
            var personRepositoryMock = new Mock<IPersonRepository>();
            personRepositoryMock.Setup(repo => repo.GetPersonByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Person)null);

            var service = new PersonService(null, personRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.GetPersonByIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetAllPersonsAsync_ReturnsListOfPersonDto_WhenPersonsExist()
        {
            // Arrange
            var personRepositoryMock = new Mock<IPersonRepository>();
            var persons = new List<Person>
    {
        new Person { PlaceOfResidence = new PlaceOfResidence() },
        new Person { PlaceOfResidence = new PlaceOfResidence() }
    };
            personRepositoryMock.Setup(repo => repo.GetAllPersonsAsync()).ReturnsAsync(persons);

            var service = new PersonService(null, personRepositoryMock.Object);

            // Act
            var result = await service.GetAllPersonsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(persons.Count, result.Count);
        }

        [Fact]
        public async Task UpdatePersonAsync_ThrowsKeyNotFoundException_WhenPersonNotFound()
        {
            // Arrange
            var personRepositoryMock = new Mock<IPersonRepository>();
            personRepositoryMock.Setup(repo => repo.GetPersonByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Person)null);

            var service = new PersonService(null, personRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.UpdatePersonAsync(Guid.NewGuid(), Guid.NewGuid(), new UpdatePersonDTO()));
        }

        [Fact]
        public async Task DeletePersonAsync_ThrowsKeyNotFoundException_WhenPersonNotFound()
        {
            // Arrange
            var personRepositoryMock = new Mock<IPersonRepository>();
            personRepositoryMock.Setup(repo => repo.GetPersonByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Person)null);

            var service = new PersonService(null, personRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.DeletePersonAsync(Guid.NewGuid(), Guid.NewGuid()));
        }
    }
}
