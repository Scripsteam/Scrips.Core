using System.Text.Json.Serialization;

namespace Scrips.Core.Enums.Scheduling
{
    /// <summary>
    ///     Is the Participant required to attend the appointment.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ParticipantRequired
    {
        /// <summary>
        ///     The participant is required to attend the appointment.
        /// </summary>
        Required,

        /// <summary>
        ///     The participant may optionally attend the appointment.
        /// </summary>
        Optional,

        /// <summary>
        ///     The participant is excluded from the appointment, and might not be informed of the appointment taking place.
        ///     (Appointment is about them, not for them - such as 2 doctors discussing results about a patient's test).
        /// </summary>
        InformationOnly
    }
}
