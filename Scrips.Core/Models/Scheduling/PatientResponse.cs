using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scrips.Core.Models.Patient;

namespace Scrips.Core.Models.Scheduling;

/// <summary>
///     Patient Response.
/// </summary>
public class PatientResponse
{
    /// <summary>
    /// Patient Response Constructor.
    /// </summary>
    public PatientResponse()
    {
        GuardianDetails = new GuardianDetails();
    }

    /// <summary>
    /// Id of the patient.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// First name of the patient.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Middle name of the patient.
    /// </summary>
    public string MiddleName { get; set; }

    /// <summary>
    /// Last name of the patient.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// MRN of the patient.
    /// </summary>
    public string Mrn { get; set; }

    /// <summary>
    /// ExternalMRN of the patient.
    /// </summary>
    public string ExternalMRN { get; set; }

    /// <summary>
    /// Birth date of the patient.
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Gender of the patient.
    /// </summary>
    public string GenderName { get; set; }

    /// <summary>
    /// Gender Id of the patient.
    /// </summary>
    public Guid Gender { get; set; }

    /// <summary>
    /// Photo Url of the patient.
    /// </summary>
    public string PhotoUrl { get; set; }

    /// <summary>
    /// Phone number of the patient.
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Email of the patient.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Time zone of the patient.
    /// </summary>
    public string TimeZone { get; set; }

    /// <summary>
    /// Identification Id of the patient.
    /// </summary>
    public string IdentificationId { get; set; }

    /// <summary>
    /// Guardian details of the patient.
    /// </summary>
    public GuardianDetails GuardianDetails { get; set; }

    /// <summary>
    /// Age of the patient.
    /// </summary>
    public string Age { get; set; }

    /// <summary>
    /// In Onboarding.
    /// </summary>
    public bool IsOnboarding { get; set; }

    /// <summary>
    /// Marital Status.
    /// </summary>
    public string MaritalStatus { get; set; }
    
    /// <summary>
    /// Marital Status Display.
    /// </summary>
    public string MaritalStatusDisplay { get; set; }

    /// <summary>
    /// Full name of the patient.
    /// </summary>
    public string Name => FirstName.Trim() +
                          $"{(!string.IsNullOrWhiteSpace(MiddleName) ? " " + MiddleName.Trim() : string.Empty)}" + " " + LastName.Trim();

    /// <summary>
    /// Is Insurance Added.
    /// </summary>
    public bool IsInsuranceAdded { get; set; }

    /// <summary>
    /// City of the patient.
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Country of the patient.
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// Address of the patient.
    /// </summary>
    public string Address { get; set; }

    public string LanguageId { get; set; }
    public string LanguageDisplay { get; set; }

    /// <summary>
    /// Full address of the patient.
    /// </summary>
    public string FullAddress => ((!string.IsNullOrWhiteSpace(Address)) ? Address.Trim() : string.Empty) + " " + ((!string.IsNullOrWhiteSpace(City)) ? City.Trim() : string.Empty) + " " + ((!string.IsNullOrWhiteSpace(Country)) ? Country.Trim() : string.Empty);

    public PatientEducationResponse PatientEducation { get; set; }
    public PatientOccupationResponse PatientOccupation { get; set; }

    [JsonExtensionData]
    public IDictionary<string, JToken> Extensions { get; set; } = new Dictionary<string, JToken>();
}