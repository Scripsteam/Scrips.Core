namespace Scrips.Core.Models.Practice;

public class ServiceLocationFeeDto
{
    public Guid? Id { get; set; }
    public LocationDto Location { get; set; }
    public double Fee { get; set; }
    public Guid? ServiceId { get; set; }
}