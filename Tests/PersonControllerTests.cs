using FinalExam.BusinessLogic.Interfaces;
using FinalExam.Controllers;
using FinalExam.Database.Interfaces;
using FinalExam.Database.Models;
using FinalExam.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;


namespace Tests
{
    public class PersonControllerTests
    {
        private readonly Mock<IPersonService> _mockPersonService;
        private readonly Mock<IPersonRepository> _mockPersonRepository;
        private readonly PersonController _controller;

        public PersonControllerTests()
        {
            _mockPersonService = new Mock<IPersonService>();
            _mockPersonRepository = new Mock<IPersonRepository>();
            _controller = new PersonController(_mockPersonService.Object, _mockPersonRepository.Object);
        }

        private void SetupUserClaims()
        {
            var userId = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task CreatePerson_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            SetupUserClaims();
            var personDto = new PersonDTO
            {
                Name = "Johny",
                LastName = "Johnas",
                Gender = "Male",
                Birthday = new DateTime(1990, 1, 1),
                PersonalCode = "1234567890",
                TelephoneNumber = "123456789",
                Email = "johnjohnas@gmail.com",
                PlaceOfResidence = new PlaceOfResidenceDTO
                {
                    City = "New York",
                    Street = "Main Street",
                    HouseNumber = "123",
                    ApartmentNumber = "456"
                }
            };

            _mockPersonService.Setup(service => service.CreatePersonAsync(personDto, It.IsAny<Guid>()))
                .ReturnsAsync(personDto);

            // Act
            var result = await _controller.CreatePerson(personDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PersonDTO>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedPerson = Assert.IsType<PersonDTO>(okResult.Value);
            Assert.Equal(personDto.Name, returnedPerson.Name);
            Assert.Equal(personDto.LastName, returnedPerson.LastName);
            Assert.Equal(personDto.Gender, returnedPerson.Gender);
            Assert.Equal(personDto.Birthday, returnedPerson.Birthday);
            Assert.Equal(personDto.PersonalCode, returnedPerson.PersonalCode);
            Assert.Equal(personDto.TelephoneNumber, returnedPerson.TelephoneNumber);
            Assert.Equal(personDto.Email, returnedPerson.Email);

            // Assert PlaceOfResidence properties
            Assert.NotNull(returnedPerson.PlaceOfResidence);
            Assert.Equal(personDto.PlaceOfResidence.City, returnedPerson.PlaceOfResidence.City);
            Assert.Equal(personDto.PlaceOfResidence.Street, returnedPerson.PlaceOfResidence.Street);
            Assert.Equal(personDto.PlaceOfResidence.HouseNumber, returnedPerson.PlaceOfResidence.HouseNumber);
            Assert.Equal(personDto.PlaceOfResidence.ApartmentNumber, returnedPerson.PlaceOfResidence.ApartmentNumber);
        }

        [Fact]
        public async Task GetPersonById_ReturnsOkResult_WhenPersonExists()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var personDto = new PersonDTO();
            _mockPersonService.Setup(service => service.GetPersonByIdAsync(personId))
                .ReturnsAsync(personDto);

            // Act
            var result = await _controller.GetPersonById(personId);

            // Assert
            var okResult = Assert.IsType<ActionResult<PersonDTO>>(result);
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public async Task GetPersonById_ReturnsNotFound_WhenPersonDoesNotExist()
        {
            // Arrange
            var personId = Guid.NewGuid();
            _mockPersonService.Setup(service => service.GetPersonByIdAsync(personId))
                .ReturnsAsync((PersonDTO)null); 

            // Act
            var result = await _controller.GetPersonById(personId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PersonDTO>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal("Person not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetAllPersons_ReturnsOkResult_WithListOfPersons()
        {
            // Arrange
            var persons = new List<PersonDTO> { new PersonDTO(), new PersonDTO() };
            _mockPersonService.Setup(service => service.GetAllPersonsAsync())
                .ReturnsAsync(persons);

            // Act
            var result = await _controller.GetAllPersons();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<PersonDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedPersons = Assert.IsType<List<PersonDTO>>(okResult.Value);
            Assert.Equal(2, returnedPersons.Count);
        }

        [Fact]
        public async Task UpdatePerson_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            SetupUserClaims();
            var personId = Guid.NewGuid();
            var updatePersonDto = new UpdatePersonDTO();

            _mockPersonService.Setup(service => service.UpdatePersonAsync(personId, It.IsAny<Guid>(), updatePersonDto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdatePerson(personId, updatePersonDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeletePerson_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            SetupUserClaims();
            var personId = Guid.NewGuid();

            _mockPersonService.Setup(service => service.DeletePersonAsync(personId, It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePerson(personId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Person deleted successfully", okResult.Value);
        }

        [Fact]
        public async Task DownloadProfilePhoto_ReturnsFileResult_WhenPhotoExists()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var person = new Person { ProfilePhoto = new byte[] { 0x01, 0x02, 0x03 } };
            _mockPersonRepository.Setup(repo => repo.GetPersonByIdAsync(personId))
                .ReturnsAsync(person);

            // Act
            var result = await _controller.DownloadProfilePhoto(personId);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("image/jpeg", fileResult.ContentType);
        }

        [Fact]
        public async Task DownloadProfilePhoto_ReturnsNotFound_WhenPhotoDoesNotExist()
        {
            // Arrange
            var personId = Guid.NewGuid();
            _mockPersonRepository.Setup(repo => repo.GetPersonByIdAsync(personId))
                .ReturnsAsync((Person)null);

            // Act
            var result = await _controller.DownloadProfilePhoto(personId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

    }
}
