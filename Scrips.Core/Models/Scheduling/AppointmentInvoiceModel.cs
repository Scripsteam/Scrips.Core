namespace Scrips.Core.Models.Scheduling;

/// <summary>
/// invoice object link with appointment
/// </summary>
public class AppointmentInvoiceModel
{
    public AppointmentInvoiceModel()
    {
        Services = new List<InvoiceDetailsDto>();
    }
    /// <summary>
    /// id
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// invoice number
    /// </summary>
    public string InvoiceNumber { get; set; }

    /// <summary>
    /// status of the invoice
    /// </summary>
    public string InvoiceStatus { get; set; }

    /// <summary>
    /// Drafted Date time
    /// </summary>
    public DateTime? DraftedOn { get; set; }

    /// <summary>
    /// invoice date
    /// </summary>
    public DateTime? InvoiceDate { get; set; }

    /// <summary>
    /// gross total
    /// </summary>
    public double GrossTotal { get; set; }

    /// <summary>
    /// total discount
    /// </summary>
    public double DiscountTotal { get; set; }

    /// <summary>
    /// net total
    /// </summary>
    public double NetTotal { get; set; }

    /// <summary>
    /// patient payable amount
    /// </summary>
    public double PatientPayable { get; set; }

    /// <summary>
    /// total deposited
    /// </summary>
    public double Deposited { get; set; }

    /// <summary>
    /// sponsor outstanding
    /// </summary>
    public double SponsorOutstanding { get; set; }

    /// <summary>
    /// patient payment status
    /// </summary>
    public string PatientPaymentStatus { get; set; }

    /// <summary>
    /// patient outstanding
    /// </summary>
    public double PatientOutstanding { get; set; }

    /// <summary>
    /// Policy Number
    /// </summary>
    public string PolicyNo { get; set; }

    /// <summary>
    /// Sponsor
    /// </summary>
    public Sponsor Sponsor { get; set; }

    /// <summary>
    /// Invoice Details List
    /// </summary>
    public List<InvoiceDetailsDto> Services { get; set; }

    /// <summary>
    /// AppointmentId
    /// </summary>
    public Guid AppointmentId { get; set; } 
}