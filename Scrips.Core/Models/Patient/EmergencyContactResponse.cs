using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrips.Core.Models.Patient
{
    public class EmergencyContactResponse
    {
        public string EmergencyContactId { get; set; }
        public string PatientId { get; set; }
        public string Relationship { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ContactNumber { get; set; }
        public bool IsProxy { get; set; }
        public RelationResponse Relation { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string Building { get; set; }
        public string Apartment { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string PhotoUrl { get; set; }
    }
}
