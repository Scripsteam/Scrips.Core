namespace Scrips.Core.Models.Billing;

public class ClonedBillingGroupDto
{
    public Guid BillingGroupId { get; set; }
    public decimal NetSum { get; set; }
    public bool IsDiscount { get; set; } = false;
    public bool CoPayIsFixed { get; set; }
    public bool CoPayIsPercent { get; set; }
    public decimal TotalPatientShare => GetTotalPatientShare();
    public decimal TotalPatientDiscount => GetTotalPatientDiscount();
    public decimal CoPayValue { get; set; }
    public decimal? MaximumCopaymentValue { get; set; }

    private decimal GetTotalPatientShare()
    {
        if (!IsDiscount)
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
        }
        return 0;
    }

    private decimal GetTotalPatientDiscount()
    {
        if (IsDiscount)
        {
            if (CoPayIsFixed)
            {
                var copay = NetSum - CoPayValue;
                return copay > 0 ? copay : 0;
            }
            else if (CoPayIsPercent)
            {
                var copay = CoPayValue * NetSum / 100;
                return copay > 0 ? copay : 0;
            }
        }
        return 0;
    }
}