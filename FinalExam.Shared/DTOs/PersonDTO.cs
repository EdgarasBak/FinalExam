using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalExam.Shared.DTOs
{
    public class PersonDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        public string PersonalCode { get; set; }

        [Required]
        public string TelephoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        public IFormFile ProfilePhoto { get; set; }

        public PlaceOfResidenceDTO PlaceOfResidence { get; set; }
    }
}
