using System;
using Scrips.Core.Models.Practice;

namespace Scrips.Core.Models.Billing;

public class ServiceLocationFeeDto
{
    public Guid? Id { get; set; }
    public LocationDto? Location { get; set; }
    public double Fee { get; set; }
    public Guid? ServiceId { get; set; }
    public bool IsArchived { get; set; }
}