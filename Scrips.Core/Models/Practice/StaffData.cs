namespace Scrips.Core.Models.Practice;

public class StaffData
{
    public string Id { get; set; }

    public bool Status { get; set; }

    public string OrganizationId { get; set; }
    public string PracticeId { get; set; }

    public Guid? PractitionerId { get; set; }

    public string ImageURL { get; set; }

    public string CountryName { get; set; }

    public string TimeZone { get; set; }

    public string PracticeName { get; set; }

    public string PracticeAddress { get; set; }
    public string Locations { get; set; }

    public string MedicationSystem { get; set; }
    public bool? HasExternalMRN { get; set; } = false;
}