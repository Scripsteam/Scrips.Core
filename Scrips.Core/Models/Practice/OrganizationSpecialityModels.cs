namespace Scrips.Core.Models.Practice;

/// <summary>
/// PROD-3336: one of the organization's specialities (PracticeManagement GET api/Specialities).
/// <see cref="HasActivePractitioner"/> is false when only archived people hold it.
/// </summary>
public class OrganizationSpecialityModel
{
    public Guid SpecialityId { get; set; }
    public string Name { get; set; }
    public bool HasActivePractitioner { get; set; }
}

/// <summary>PROD-3336: one of the signed-in user's specialities (PracticeManagement GET api/Specialities/Mine).</summary>
public class UserSpecialityModel
{
    public Guid SpecialityId { get; set; }
    public string Name { get; set; }
}

/// <summary>
/// PROD-3336: an active person who holds a speciality (PracticeManagement GET api/Specialities/{id}/Practitioners).
/// <see cref="PractitionerId"/> is the AddDoctor id (<see cref="Kind"/> "doctor") or the AddStaff id ("staff");
/// <see cref="UserId"/> is null when the person has no login.
/// </summary>
public class SpecialityPractitionerModel
{
    public Guid PractitionerId { get; set; }
    public string Name { get; set; }
    public string Kind { get; set; }
    public Guid? UserId { get; set; }
}
