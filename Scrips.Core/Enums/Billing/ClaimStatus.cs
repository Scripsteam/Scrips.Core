using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Scrips.Core.Enums.Billing;

/// <summary>
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ClaimStatusType
{
    [Display(Name = "Initial")]
    Initial,

    [Display(Name = "Hold")]
    Hold,

    [Display(Name = "Reviewed")]
    Reviewed,

    [Display(Name = "Submitted")]
    Submitted,

    [Display(Name = "Resubmitted")]
    Resubmitted,

    [Display(Name = "Remitted")]
    Remitted,

    [Display(Name = "Revised & Reviewed")]
    RevisedAndReviewed,

    [Display(Name = "Reconciled")]
    Reconciled,

    [Display(Name = "Denial Addressed")]
    DenialAddressed
}