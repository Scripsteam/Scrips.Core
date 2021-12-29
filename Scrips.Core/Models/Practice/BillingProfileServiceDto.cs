using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Practice
{
    public class BillingProfileServiceDto
    {
        public BillingProfileServiceDto()
        {
            VisitFees = new List<ServiceLocationFeeDto>();
        }
        public Guid? Id { get; set; }
        public ProcedureCodeDto ProcedureCode { get; set; }
        public List<ServiceLocationFeeDto> VisitFees { get; set; }

    }
}
