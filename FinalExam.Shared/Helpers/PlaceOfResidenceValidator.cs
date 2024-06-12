using FinalExam.Shared.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalExam.Shared.Helpers
{
    public class PlaceOfResidenceValidator : AbstractValidator<PlaceOfResidenceDTO>
    {
        public PlaceOfResidenceValidator()
        {
            RuleFor(pr => pr.City)
                .NotEmpty().WithMessage("City is required.")
                .Must(city => !ContainsDigitsOrSpecialCharacters(city)).WithMessage("City cannot contain numbers or special characters.");

            RuleFor(pr => pr.Street)
                .NotEmpty().WithMessage("Street is required.")
                .Must(street => street.Contains(" ")).WithMessage("Street must contain at least one space.")
                .Matches(@"^[a-zA-ZĄČĘĖĮŠŲŪŽąčęėįšųūž]+( [a-zA-ZĄČĘĖĮŠŲŪŽąčęėįšųūž]+)*$").WithMessage("Invalid street format.");

            RuleFor(pr => pr.HouseNumber)
                .NotEmpty().WithMessage("House number is required.")
                .Matches(@"^[0-9]+[A-Za-z]?$").WithMessage("House number must be alphanumeric.");

            RuleFor(pr => pr.ApartmentNumber)
                .Matches(@"^[A-Za-z0-9\s]*$").WithMessage("Apartment number must contain only letters, numbers.");
        }

        private static bool ContainsDigitsOrSpecialCharacters(string input)
        {
            return input.Any(char.IsDigit) || !input.All(char.IsLetterOrDigit);
        }
    }
}
