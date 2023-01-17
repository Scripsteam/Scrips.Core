using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Billing
{
    public class ProviderBillingProfileDto
    {
        public Guid ProviderId { get; set; }
        public List<BillingProfileDto>? BillingProfiles { get; set; }
    }
}