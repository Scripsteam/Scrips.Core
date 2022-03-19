using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Scrips.Core.Models.Patient
{
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
        public string PatientMrn { get; set; }
        public string FrontIdCardUrl { get; set; }
        public string BackIdCardUrl { get; set; }
        public UpdateIdenitification Identification { get; set; }
        public UpdateGuardianIdenitification GuardianIdenitification { get; set; }
        public UpdatePatientContactRequest UpdatePatientContactRequest { get; set; }
        public List<EmergencyContactResponse> EmergencyContactResponse { get; set; }
        public List<HealthInsuranceResponse> HealthInsuranceResponse { get; set; }
        public List<PatientCorporateResponse> CorporateAgreementResponse { get; set; }
                
        [JsonExtensionData]
        public IDictionary<string, JToken> Extensions { get; set; } = new Dictionary<string, JToken>();
    }
}
