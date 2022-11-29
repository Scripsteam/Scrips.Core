using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Scrips.Core.Enums.Billing;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InvoiceUpdateType
{
    /// <summary>
    /// The type of update to use 
    /// when editing an invoice 
    /// before the claim is confirmed
    /// </summary>
    [Display(Name = "Primary")]
    Primary,

    /// <summary>
    /// The type of update to use 
    /// when editing an invoice 
    /// during a claim confirmation
    /// </summary>
    [Display(Name = "PreClaim")]
    PreClaim
}