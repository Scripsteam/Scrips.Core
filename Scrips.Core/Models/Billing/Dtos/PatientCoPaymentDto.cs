using System.Collections.Generic;

namespace Scrips.Core.Models.Billing.Dtos
{
    public class PatientCoPaymentDto
    {
        public List<BillingGroupDto> BillingGroups { get; set; }

        public string CoPaymentType { get; set; }
    }
}
