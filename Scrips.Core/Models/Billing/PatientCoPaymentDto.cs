using System.Collections.Generic;

namespace Scrips.Core.Models.Billing;

public class PatientCoPaymentDto
{
    public List<BillingGroupDto> BillingGroups { get; set; }

    public string CoPaymentType { get; set; }
}