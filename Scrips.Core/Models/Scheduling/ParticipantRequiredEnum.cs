namespace Scrips.Core.Models.Scheduling
{
    public enum ParticipantRequiredEnum
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
