using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Scrips.Core.Enums.Billing;

/// <summary>
/// Remittance Advice Allocation Type Enum
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RemittanceAdviceAllocationType
{

    [Display(Name = "Auto-allocated")]
    AutoAllocated,

    [Display(Name = "File")]
    File
}