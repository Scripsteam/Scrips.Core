using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Scrips.Core.Enums.Billing;

/// <summary>
/// Remittance Advice Status Enum
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RemittanceAdviceStatus
{
    [Display(Name = "Pending")]
    Pending,

    [Display(Name = "Processed")]
    Processed
}