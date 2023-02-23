namespace Scrips.Core.Models.Billing;

public class InvoiceLineItemDxPointer
{
    public Guid InvoiceId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid DiagnosisId { get; set; }
    public InvoiceLineItemDxPointerCode DxPointerCode { get; set; }
    public string Note { get; set; }
    public bool? IsDeleted { get; set; }
}