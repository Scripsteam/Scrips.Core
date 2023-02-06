namespace Scrips.Core.Models.Billing;

public class BaseCalculatePrice
{
    public string PaymentType { get; set; }
    public string CompanyCode { get; set; }

    public List<IndividualServiceDto> IndividualServices { get; set; }
    public PatientCoPaymentDto PatientCoPayment { get; set; }
}