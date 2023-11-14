namespace Scrips.Core.Models.Billing;

public class BillingGroupDto
{
    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public decimal? MaximumCopayment { get; set; }
}