namespace Scrips.Core.Models.Practice;

public class DoctorData
{
    public string Id { get; set; }

    public string PracticeId { get; set; }

    public string OrganizationId { get; set; }

    public string MedicationSystem { get; set; }

    public bool Status { get; set; }

    public bool IsOnboarded { get; set; }
    public string ImageURL { get; set; }
         
    public Speciality Speciality { get; set; }

    public string CountryName { get; set; }

    public string TimeZone { get; set; }
    public string Locations { get; set; }
}