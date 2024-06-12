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
    public class UpdatePersonValidator : AbstractValidator<UpdatePersonDTO>
    {
        public UpdatePersonValidator()
        {
            RuleFor(p => p.Name)
            .MaximumLength(50).When(p => !string.IsNullOrWhiteSpace(p.Name))
            .Matches(@"^[\p{L}]{2,50}$")
            .When(p => !string.IsNullOrWhiteSpace(p.Name))
            .WithMessage("Name must be 2-50 characters long and contain only letters.");

            RuleFor(p => p.LastName)
                .MaximumLength(50).When(p => !string.IsNullOrWhiteSpace(p.LastName))
                .Matches(@"^[\p{L}]{2,50}$")
                .When(p => !string.IsNullOrWhiteSpace(p.LastName))
                .WithMessage("Last name must be 2-50 characters long and contain only letters.");

            RuleFor(p => p.Gender)
                .Must(g => g == "Male" || g == "Female" || g == "Other")
                .When(p => !string.IsNullOrWhiteSpace(p.Gender));

            RuleFor(p => p.Birthday)
                .LessThan(DateTime.Now)
                .When(p => p.Birthday.HasValue);

            RuleFor(p => p.PersonalCode)
                .Length(11).When(p => !string.IsNullOrWhiteSpace(p.PersonalCode))
                .Must(IsValidLithuanianPersonalCode)
                .When(p => !string.IsNullOrWhiteSpace(p.PersonalCode))
                .WithMessage("Invalid Lithuanian Personal Code.");

            RuleFor(p => p.TelephoneNumber)
                .Matches(@"^\+370\d{8}$")
                .When(p => !string.IsNullOrWhiteSpace(p.TelephoneNumber))
                .WithMessage("Lithuanian telephone number must start with '+370' followed by 8 digits.");

            RuleFor(p => p.Email)
                .EmailAddress().When(p => !string.IsNullOrWhiteSpace(p.Email));


            RuleFor(p => p.PlaceOfResidence)
                .SetValidator(new UpdatePlaceOfResidenceValidator())
                .When(p => p.PlaceOfResidence != null);
        }
        public static bool IsValidLithuanianPersonalCode(string personalCode)
        {
            if (personalCode == null)
            {
                return false;
            }

            if (personalCode.Length != 11)
            {
                return false;
            }

            if (!Regex.IsMatch(personalCode, @"^\d{11}$"))
            {
                return false;
            }

            int centuryAndGender = int.Parse(personalCode.Substring(0, 1));
            int year = int.Parse(personalCode.Substring(1, 2));
            int month = int.Parse(personalCode.Substring(3, 2));
            int day = int.Parse(personalCode.Substring(5, 2));
            int checkDigit = int.Parse(personalCode.Substring(10, 1));

            try
            {
                int fullYear = GetFullYear(centuryAndGender, year);
                var birthDate = new DateTime(fullYear, month, day);
            }
            catch
            {
                return false;
            }

            if (checkDigit != CalculateCheckDigit(personalCode))
            {
                return false;
            }

            return true;
        }
        private static int GetFullYear(int centuryAndGender, int year)
        {
            if (centuryAndGender >= 1 && centuryAndGender <= 2)
            {
                return 1800 + year;
            }
            if (centuryAndGender >= 3 && centuryAndGender <= 4)
            {
                return 1900 + year;
            }
            if (centuryAndGender >= 5 && centuryAndGender <= 6)
            {
                return 2000 + year;
            }
            if (centuryAndGender >= 7 && centuryAndGender <= 8)
            {
                return 2100 + year;
            }
            throw new ArgumentException("Invalid century and gender indicator in personal code.");
        }
        private static int CalculateCheckDigit(string personIdCode)
        {
            int[] weights1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1 };
            int[] weights2 = { 3, 4, 5, 6, 7, 8, 9, 1, 2, 3 };

            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += (personIdCode[i] - '0') * weights1[i];
            }

            int remainder = sum % 11;
            if (remainder == 10)
            {
                sum = 0;
                for (int i = 0; i < 10; i++)
                {
                    sum += (personIdCode[i] - '0') * weights2[i];
                }
                remainder = sum % 11;
                if (remainder == 10)
                {
                    remainder = 0;
                }
            }

            return remainder;
        }
    }
}
