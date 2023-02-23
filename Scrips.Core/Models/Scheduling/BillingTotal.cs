namespace Scrips.Core.Models.Scheduling;

public class BillingTotal
{
    /// <summary>
    /// Gross Total.
    /// </summary>
    public decimal? GrossTotal { get; set; }

    /// <summary>
    /// Discount Total.
    /// </summary>
    public decimal? DiscountTotal { get; set; }

    /// <summary>
    /// Net Total.
    /// </summary>
    public decimal? NetTotal { get; set; }

    /// <summary>
    /// Patient Share Total.
    /// </summary>
    public decimal? PatientShareTotal { get; set; }

    /// <summary>
    /// Insurance Amount Total.
    /// </summary>
    public decimal? InsuranceAmountTotal { get; set; }
}