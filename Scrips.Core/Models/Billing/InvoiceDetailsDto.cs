namespace Scrips.Core.Models.Billing;

public class InvoiceDetailsDto
{
    public Guid Id { get; set; }
    public Guid? InvoiceId { get; set; }
    public Guid? ServiceId { get; set; }
    public DateTime ServiceAddedDate { get; set; }
    public Guid? ServicePlacedBy { get; set; }
    public bool ServiceIncluded { get; set; }
    public bool ServiceExcluded { get; set; }
    public bool ServiceNeedsApproval { get; set; }
    public int Quantity { get; set; }
    public double GrossAmount { get; set; }
    public double DiscountAmount { get; set; }
    public double NetAmount { get; set; }
    public double SponsorAmount { get; set; }
    public double PatientAmount { get; set; }
    public DateTime? CreatedOn { get; set; }
    public Guid? CreatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public Guid? UpdatedById { get; set; }
    public bool? IsArchived { get; set; }
    public string DXPointer { get; set; }
    public string AuthCode { get; set; }
    public DateTime? ValidityStartDate { get; set; }
    public DateTime? ValidityEndDate { get; set; }
    public List<InvoiceLineItemDxPointer> DxPointers { get; set; }
    public ProcedureCodeModel ProcedureCode { get; set; }
    public Guid BillingGroupId { get; set; }
    public string DenialReason { get; set; }
    public double? AmountToResubmit { get; set; }
    public double Payment { get; set; }
    public double Outstanding { get; set; }
    public string BillingGroupSystem { get; set; }
    public string BillingGroupCategory { get; set; }
    public bool? IsIndividualAdded { get; set; }
}