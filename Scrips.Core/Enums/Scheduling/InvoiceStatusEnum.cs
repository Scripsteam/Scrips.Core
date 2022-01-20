using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Scrips.Core.Enums.Scheduling
{
    /// <summary>
    /// Invoice Enum
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum InvoiceStatusEnum
    {
        [Display(Name = "Draft")]
        Draft,

        [Display(Name = "Issued")]
        Issued,

        [Display(Name = "Balanced")]
        Balanced,

        [Display(Name = "Canceled")]
        Canceled,

        [Display(Name = "Claimed")]
        Claimed,

        [Display(Name = "Reclaimed")]
        Reclaimed
    }
}
