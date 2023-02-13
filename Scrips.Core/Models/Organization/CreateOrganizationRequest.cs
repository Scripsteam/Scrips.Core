namespace Scrips.Core.Models.Organization;

public class CreateOrganizationRequest
{
    public string TenantId { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; }
    public Guid CreatedBy { get; set; }
    public string LicenseType { get; set; }
    public string LicenseNumber { get; set; }
    public DateTime? LicenseExpirationDate { get; set; }
    public string LicenseIssuer { get; set; }
    public string LogoUrl { get; set; }
    public string FrontLicenseIdURL { get; set; }
    public string BackLicenseIdURL { get; set; }
    public string BrandingUrl { get; set; }
    public string NameArabic { get; set; }
    public string Jurisdiction { get; set; }
    public Guid? OwnerId { get; set; }
}