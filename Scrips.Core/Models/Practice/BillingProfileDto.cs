using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Practice;

public class BillingProfileDto
{
    public BillingProfileDto()
    {
        Services = new List<BillingProfileServiceDto>();
    }
    public Guid ProviderId { get; set; }
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public List<BillingProfileServiceDto> Services { get; set; }
    public double TotalCost { get; set; }
}