using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Scrips.Core.Enums.Patient
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EncounterStatus
    {
        [Display(Name = "Initial")]
        Initial,

        [Display(Name = "Draft")]
        Draft,

        [Display(Name = "Completed")]
        Completed
    }
}
