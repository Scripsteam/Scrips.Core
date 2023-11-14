namespace Scrips.Core.Models.Billing;

public class ClonedBillingGroupDto
{
    public Guid BillingGroupId { get; set; }
    public decimal NetSum { get; set; }
    public bool CoPayIsFixed { get; set; }
    public bool CoPayIsPercent { get; set; }
    public decimal TotalPatientShare => GetTotalPatientShare();
    public decimal CoPayValue { get; set; }
    public decimal? MaximumCopaymentValue { get; set; }

    private decimal GetTotalPatientShare()
    {
        if (CoPayIsFixed)
        {
            var copay = Math.Min(NetSum, CoPayValue);
            return MaximumCopaymentValue.HasValue ? Math.Min(copay, MaximumCopaymentValue.Value) : copay;
        }
        else if (CoPayIsPercent)
        {
            var copay = CoPayValue * NetSum / 100;
            return MaximumCopaymentValue.HasValue ? Math.Min(copay, MaximumCopaymentValue.Value) : copay;
        }
        return 0;
    }
}