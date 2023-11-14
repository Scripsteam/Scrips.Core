namespace Scrips.Core.Models.Patient;

public class PatientBillingGroupModel
{
    public Guid BillingGroupId { get; set; }
    public string Category { get; set; }
    public decimal Value { get; set; }
    public decimal? MaximumCopayment { get; set; }
}