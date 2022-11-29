using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Scrips.Core.Enums.Billing;

/// <summary>
/// Invoice Claim Request Status
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ClaimRequestStatus
{
    [Display(Name = "Sending")]
    Sending,

    [Display(Name = "Submitted")]
    Submitted,

    [Display(Name = "Error")]
    Error,

    [Display(Name = "Connection Error")]
    ConnectionError,

    [Display(Name = "Ready for Submission")]
    ReadyForSubmission,

    [Display(Name = "Denial Addressed")]
    DenialAddressed
}