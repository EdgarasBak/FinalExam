using FinalExam.Shared.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FinalExam.Shared.Helpers
{
    public class UpdatePlaceOfResidenceValidator : AbstractValidator<UpdatePlaceOfResidenceDTO>
    {
        public UpdatePlaceOfResidenceValidator()
        {
            RuleFor(pr => pr.City)
                .Must(city => string.IsNullOrEmpty(city) || !ContainsDigitsOrSpecialCharacters(city))
                .WithMessage("City cannot contain numbers or special characters.");

            RuleFor(pr => pr.Street)
                .Must(street => string.IsNullOrEmpty(street) || Regex.IsMatch(street, @"^\p{L}+(\s\p{L}+)*$"))
                .WithMessage("Invalid street format. Street must contain at least one space.");

            RuleFor(pr => pr.HouseNumber)
                .Matches(@"^[0-9]+[A-Za-z]?$").When(pr => !string.IsNullOrEmpty(pr.HouseNumber))
                .WithMessage("House number must be alphanumeric.");

            RuleFor(pr => pr.ApartmentNumber)
                .Matches(@"^[A-Za-z0-9\s]*$").When(pr => !string.IsNullOrEmpty(pr.ApartmentNumber))
                .WithMessage("Apartment number must contain only letters, numbers, and spaces.");
        }

        private static bool ContainsDigitsOrSpecialCharacters(string input)
        {
            return input.Any(char.IsDigit) || !input.All(char.IsLetterOrDigit);
        }
    }
}
