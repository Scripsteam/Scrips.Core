namespace Scrips.Core.Models.Billing;

public class BillingProfileDto
{
    public bool IsProviderFee { get; set; }
    public Guid ProviderId { get; set; }
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public bool? IsUpdated { get; set; }
    public bool? IsDeleted { get; set; }
    public List<BillingProfileServiceDto>? Services { get; set; }

    public double? TotalCost { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? CreatedOn { get; set; }
    public Guid? CreatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public Guid? UpdatedById { get; set; }
}