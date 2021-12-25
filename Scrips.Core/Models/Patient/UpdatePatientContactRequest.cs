using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrips.Core.Models.Patient
{
    public class UpdatePatientContactRequest
    {
        public UpdatePatientContactRequest()
        {
            PatientAddress = new List<PatientContactAddressModel>();
        }
        public string PatientId { get; set; }

        public string Ownership { get; set; }

        public string Email { get; set; }

        public string PrimaryContact { get; set; }

        public string SecondayContact { get; set; }

        public int IsPreferrable { get; set; }

        public List<PatientContactAddressModel> PatientAddress { get; set; }
    }
}
