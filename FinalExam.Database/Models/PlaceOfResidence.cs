using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalExam.Database.Models
{
    public class PlaceOfResidence
    {
        public Guid Id { get; set; } 
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string ApartmentNumber { get; set; }
        public Guid PersonId { get; set; } 
        public Person Person { get; set; } 
    }
}
