namespace Scrips.Core.Models.Billing;

public class Balance
{
    public string? Subject { get; set; }
    public double Amount { get; set; }
    public string? FinancialEventStatus { get; set; }
}