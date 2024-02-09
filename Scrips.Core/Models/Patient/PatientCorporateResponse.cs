namespace Scrips.Core.Models.Patient;

public class PatientCorporateResponse
{
    public PatientCorporateResponse()
    {
        BillingGroups = new List<PatientBillingGroupModel>();
    }
    public string CorporateId { get; set; }
    public string PatientId { get; set; }
    public ProviderInsuranceModel CorporateProvider { get; set; }
    public string PolicyNumber { get; set; }
    public DateTime? EligiblityStartDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsDeleted { get; set; }
    public MasterResponse InsurerName { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public string FrontPhotoUrl { get; set; }
    public string BackPhotoUrl { get; set; }
    public string CopaymentType { get; set; }
    public bool IsReviewed { get; set; }
    public bool? IsVerified { get; set; }
    public string AgreementName { get; set; }
    public string AgreementId { get; set; }
    public Guid CorporateProviderId {  get; set; }
    public List<PatientBillingGroupModel> BillingGroups { get; set; }
}