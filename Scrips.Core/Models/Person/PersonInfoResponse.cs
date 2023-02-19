namespace Scrips.Core.Models.Person;

public class PersonInfoResponse
{
    public Guid OrganizationId { get; set; }
    public Guid PracticeId { get; set; }

    public Guid PatientId { get; set; }

    public string OrganizationName { get; set; }
    public string BrandingUrl { get; set; }

    public string ImageURL { get; set; }

    public bool Status { get; set; }
    public DateTime BirthDate { get; set; }
    public string Age { get; set; }
}