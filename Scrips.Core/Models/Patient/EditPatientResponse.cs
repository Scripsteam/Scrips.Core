using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Patient
{
    public class EditPatientResponse
    {
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
        public string Gender { get; set; }
        public MasterResponse GenderName { get; set; }
        public MasterResponse LanguageName { get; set; }
        public string TimeZone { get; set; }
        public string Language { get; set; }
        public string MaritalStatus { get; set; }
        public string PhotoURL { get; set; }
        public string IDFrontImage { get; set; }
        public string IDBackImage { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdated { get; set; }
        public string PatientMRN { get; set; }
        public string FrontIdCardURL { get; set; }
        public string BackIdCardURL { get; set; }
        public UpdateIdenitification Identification { get; set; }
        public UpdateGuardianIdenitification GuardianIdenitification { get; set; }
        public UpdatePatientContactRequest UpdatePatientContactRequest { get; set; }
        public List<EmergencyContactResponse> EmergencyContactResponse { get; set; } //
        public List<HealthInsuranceResponse> HealthInsuranceResponse { get; set; } //
        public List<PatientCorporateResponse> CorporateAgreementResponse { get; set; }
    }
}
