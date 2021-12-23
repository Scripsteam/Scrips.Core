using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrips.Core.Models.Patient
{
    public class PatientContactAddressModel
    {
        public string ContactId { get; set; }
        public string Building { get; set; }
        public string Apartment { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string LocationName { get; set; }
        public string Notes { get; set; }
    }
}
