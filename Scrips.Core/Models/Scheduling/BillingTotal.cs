namespace Scrips.Core.Models.Scheduling;

public class BillingTotal
{
    /// <summary>
    /// 
    /// </summary>
    public decimal? GrossTotal { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal? DiscountTotal { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal? NetTotal { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal? PatientShareTotal { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal? InsuranceAmountTotal { get; set; }
}