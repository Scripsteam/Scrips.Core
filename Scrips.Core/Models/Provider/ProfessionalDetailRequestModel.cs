namespace Scrips.Core.Models.Provider;

public class ProfessionalDetailRequestModel
{
    public Guid? Id { get; set; }
    public Guid PractitionerId { get; set; }
    public Guid PractitionerRoleId { get; set; }
    public Guid? LicenceAuthorityId { get; set; }
    public string LicenceNo { get; set; }
    public string ProfessionalStatement { get; set; }
    public DateTime? ExpirationDate { get; set; }
    /// <summary>Provider signature image URL (blob). Round-trips to the entity's DoctorSignatureUrl column.</summary>
    public string DoctorSignatureUrl { get; set; }
    public Guid DoctorSetupId { get; set; }
    public List<Communication> Languages { get; set; }
    public List<SpecialitySkillsModel> Speciality { get; set; }
    public List<SpecialitySkillsModel> Skills { get; set; }
    public List<ProviderReasonVisitModel> ProviderReasonVisits { get; set; }
}