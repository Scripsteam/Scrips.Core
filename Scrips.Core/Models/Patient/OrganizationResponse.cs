namespace Scrips.Core.Models.Patient;

public class OrganizationResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string OrganizationName { get; set; }
    public string BrandingUrl { get; set; }
}