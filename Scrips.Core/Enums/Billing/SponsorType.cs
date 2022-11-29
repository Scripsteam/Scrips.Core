using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Scrips.Core.Enums.Billing;

/// <summary>
/// Sponsor Type Enum
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SponsorType
{
    /// <summary>
    /// Self-Pay Type
    /// </summary>
    [Display(Name = "Self-Pay")]
    SelfPay,

    /// <summary>
    /// Insurance Type
    /// </summary>
    [Display(Name = "Insurance")]
    Insurance,

    /// <summary>
    /// Corporate Type
    /// </summary>
    [Display(Name = "Corporate")]
    Corporate
}