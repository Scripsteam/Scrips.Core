namespace Scrips.Core.Models.Organization;

public class OrganizationInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string CountryName { get; set; }
    public bool Status { get; set; }
    public bool IsOnboarded { get; set; }
    public string ImageURL { get; set; }
    public string TimeZone { get; set; }
    public string Locations { get; set; }
    public string MedicationSystem { get; set; }

}