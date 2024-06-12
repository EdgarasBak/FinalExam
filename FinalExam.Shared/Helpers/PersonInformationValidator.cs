using FinalExam.Shared.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FinalExam.Shared.Helpers
{
    public class PersonInformationValidator : AbstractValidator<PersonDTO>
    {
        public PersonInformationValidator()
        {
            RuleFor(pi => pi.Name)
                .NotEmpty()
                .Matches(@"^[\p{L}]{2,50}$")
                .WithMessage("Name must be 2-50 characters long and contain only letters.");

            RuleFor(pi => pi.LastName)
                .NotEmpty()
                .Matches(@"^[\p{L}]{2,50}$")
                .WithMessage("Last name must be 2-50 characters long and contain only letters.");

            RuleFor(pi => pi.Birthday)
                .NotEmpty()
                .LessThan(DateTime.Now)
                .WithMessage("Birthday must be a past date.");            

            RuleFor(pi => pi.Gender)
               .NotEmpty()
               .Must(gender => gender == "Male" || gender == "Female")
               .WithMessage("Invalid gender.");

            RuleFor(pi => pi.PersonalCode)
                .NotEmpty()
                .Must(IsValidLithuanianPersonalCode)
                .WithMessage("Invalid Lithuanian Personal Identification Code.");

            RuleFor(pi => pi.TelephoneNumber)
                .NotEmpty().WithMessage("Telephone number is required.")
                .Matches(@"^\+370\d{8}$").WithMessage("Lithuanian telephone number must start with '+370' followed by 8 digits.");

            RuleFor(pi => pi.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(pi => pi.ProfilePhoto)
                .Empty().When(pi => pi.ProfilePhoto == null) 
                .WithMessage("The ProfilePhoto field is required.");

            RuleFor(pi => pi.PlaceOfResidence)
                .SetValidator(new PlaceOfResidenceValidator());
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
            char genderIndicator = GetGenderIndicator(centuryAndGender);
            char providedGender = personalCode[0] % 2 == 0 ? 'F' : 'M'; 

            if (genderIndicator != providedGender)
            {
                return false; 
            }

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

        private static char GetGenderIndicator(int centuryAndGender)
        {
            switch (centuryAndGender)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                    return 'M';
                case 2:
                case 4:
                case 6:
                case 8:
                    return 'F';
                default:
                    throw new ArgumentException("Invalid century and gender indicator in personal code.");
            }
        }
        private static int GetFullYear(int centuryAndGender, int year) //Calcu the full birth year based on century and year digits in the personal code
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
        private static int CalculateCheckDigit(string personIdCode) //computes the chech digits
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

