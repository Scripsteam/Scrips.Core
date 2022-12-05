using System.Text.Json.Serialization;

namespace Scrips.Core.Models.Practice;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EnumAppointmentProfileType
{
    /// <summary>
    /// 
    /// </summary>
    ROUTINE,
    /// <summary>
    /// 
    /// </summary>
    FOLLOWUP
}