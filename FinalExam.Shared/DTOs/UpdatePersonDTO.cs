using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FinalExam.Shared.DTOs
{
    public class UpdatePersonDTO
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string? PersonalCode { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? Email { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IFormFile? ProfilePhoto { get; set; }
        public UpdatePlaceOfResidenceDTO? PlaceOfResidence { get; set; }
    }
}
