namespace Scrips.Core.Models.Patient;

public class GuardianDto
{
    public Guid Id { get; set; }
    public Guid? PatientId { get; set; }
    public Guid RelationId { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public Guid? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public Guid? Idtype { get; set; }
    public string Idnumber { get; set; }
    public DateTime? IdexpirationDate { get; set; }
    public string IdfrontImage { get; set; }
    public string IdbackImage { get; set; }
    public bool IsDeleted { get; set; }
    public bool? IsActive { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedTs { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedTs { get; set; }
    public Guid? UserId { get; set; }
        
    public string Age { get; set; }
    public Guid? OrganizationId { get; set; }
    public string OrganizationName { get; set; }
    public string BrandingUrl { get; set; }
    public Guid? PracticeId { get; set; }
}