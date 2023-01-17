using System;
using System.Collections.Generic;
using Practice.Aggregator.Models.Practice;

namespace Scrips.Core.Models.Billing
{
    public class BillingProfileServiceDto
    {
        public Guid? Id { get; set; }
        public ProcedureCodeDto? ProcedureCode { get; set; }
        public List<ServiceLocationFeeDto>? VisitFees { get; set; }
        public bool? IsDeleted { get; set; }
        public Guid BillingGroupId { get; set; }
    }
}