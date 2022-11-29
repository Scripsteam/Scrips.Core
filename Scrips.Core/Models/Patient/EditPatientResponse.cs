using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Scrips.Core.Models.Patient;

public class EditPatientResponse
{
    public EditPatientResponse()
    {
        EmergencyContactResponse = new List<EmergencyContactResponse>();
        HealthInsuranceResponse = new List<HealthInsuranceResponse>();
        CorporateAgreementResponse = new List<PatientCorporateResponse>();
    }

    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsAdult { get; set; }
    public string OrganizationId { get; set; }
    public string PatientId { get; set; }
    public string PracticeId { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Age { get; set; }
    public string Gender { get; set; }
    public MasterResponse GenderName { get; set; }
    public MasterResponse LanguageName { get; set; }
    public string TimeZone { get; set; }
    public string Language { get; set; }
    public string MaritalStatus { get; set; }
    public string PhotoUrl { get; set; }
    public string IdFrontImage { get; set; }
    public string IdBackImage { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastUpdated { get; set; }
    public string UpdatedBy { get; set; }
    public string PatientMrn { get; set; }
    public string FrontIdCardUrl { get; set; }
    public string BackIdCardUrl { get; set; }
    public bool? IsMissingData { get; set; }
    public UpdateIdenitification Identification { get; set; }
    public UpdateGuardianIdenitification GuardianIdenitification { get; set; }
    public UpdatePatientContactRequest UpdatePatientContactRequest { get; set; }
    public List<EmergencyContactResponse> EmergencyContactResponse { get; set; }
    public List<HealthInsuranceResponse> HealthInsuranceResponse { get; set; }
    public List<PatientCorporateResponse> CorporateAgreementResponse { get; set; }
    public bool IsMerged { get; set; }
    public bool? IsArchived { get; set; }
    public List<PatientDetail> ChildPatients { get; set; }
    public PatientDetail ParentPatient { get; set; }

    [JsonExtensionData]
    public IDictionary<string, JToken> Extensions { get; set; } = new Dictionary<string, JToken>();
}

public class PatientDetail
{
    public Guid PatientId { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public Guid Gender { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime BirthDate { get; set; }
    public Guid? MaritalStatus { get; set; }
    public Guid? LanguageId { get; set; }
    public string Photo { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid PracticeId { get; set; }
    public string Umpid { get; set; }
    public string Mrn { get; set; }
    public string TimeZone { get; set; }
    public Guid? Ssn { get; set; }
    public bool IsDeleted { get; set; }
    public bool? IsActive { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedTs { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedTs { get; set; }

}