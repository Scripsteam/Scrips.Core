using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrips.Core.Models.Patient
{
    public class PatientBillingGroupModel
    {
        public Guid BillingGroupId { get; set; }
        public string Category { get; set; }
        public decimal Value { get; set; }
    }
}
