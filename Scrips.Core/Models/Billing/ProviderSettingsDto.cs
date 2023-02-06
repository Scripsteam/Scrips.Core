namespace Scrips.Core.Models.Billing;

public class ProviderSettingsDto
{
    public int ServicesCount { get; set; }
    public Guid ProviderId { get; set; }
    public List<BillingProfileDto>? BillingProfiles { get; set; }
}