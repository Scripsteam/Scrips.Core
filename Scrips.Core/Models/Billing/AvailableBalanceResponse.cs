using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Billing
{
    public class AvailableBalanceResponse
    {
        public Guid PatientId { get; set; }
        public List<Balance>? Balance { get; set; }
    }
}