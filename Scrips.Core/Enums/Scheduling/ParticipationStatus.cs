using System.Text.Json.Serialization;

namespace Scrips.Core.Enums.Scheduling;

/// <summary>
///     The Participation status of an appointment.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ParticipationStatus
{
    /// <summary>
    ///     The participant has accepted the appointment.
    /// </summary>
    Accepted,

    /// <summary>
    ///     The participant has declined the appointment and will not participate in the appointment.
    /// </summary>
    Declined,

    /// <summary>
    ///     The participant has tentatively accepted the appointment.
    ///     This could be automatically created by a system and requires further processing before it can be accepted.
    ///     There is no commitment that attendance will occur.
    /// </summary>
    Tentative,

    /// <summary>
    ///     The participant needs to indicate if they accept the appointment by changing this status to one of the other
    ///     statuses.
    /// </summary>
    NeedsAction
}