namespace Scrips.Core.Models.Practice;

public class DoctorSetupDetails
{
    public Guid OrganizationId { get; set; }
    public Guid DoctorSetupId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailId { get; set; }

    public bool IsSuperAdmin { get; set; }
    public string SpecialityName { get; set; }
    public Guid SpecialityId { get; set; }
    public int ServiceCategoryId { get; set; }
    public int ServiceTypeId { get; set; }
    public bool? InvitationStatus { get; set; }
    public List<Doctors> PracticeList { get; set; }

    public DateTime? IsLastLogin { get; set; }

    public Speciality Speciality { get; set; }
    public List<Speciality> DoctorSpecialities { get; set; }
    public List<Guid> DoctorSpecialityIds { get; set; }
    public string DoctorSpecialitiesIds { get; set; }
    public string PhotoURL { get; set; }
    public string PracticeName { get; set; }
    public IssueObject Issue { get; set; }
}
public class Doctors
{
    public string PracticeName { get; set; }
}