using System.Text.Json.Serialization;
// ReSharper disable InconsistentNaming

namespace Scrips.Core.Enums.Scheduling;

/// <summary>
///     The style of appointment or patient that has been booked in the slot (not service type)
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppointmentType
{
    /// <summary>
    ///     A follow up visit from a previous appointment
    /// </summary>
    FOLLOWUP,

    /// <summary>
    ///     Routine appointment - default if not valued
    /// </summary>
    ROUTINE,

    /// <summary>
    ///     A previously unscheduled walk-in visit
    /// </summary>
    WALKIN
}