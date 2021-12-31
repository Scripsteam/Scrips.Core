namespace Scrips.Core.Models.Scheduling
{
    /// <summary>
    ///     The style of appointment or patient that has been booked in the slot (not service type)
    /// </summary>
    public class AppointmentType
    {
        /// <summary>
        ///     Code defined by a terminology system
        /// </summary>
        public Enums.Scheduling.AppointmentType Code { get; set; }

        /// <summary>
        ///     A human language representation of the concept as seen/selected/uttered by the user who entered the data
        ///     and/or which represents the intended meaning of the user.
        /// </summary>
        public string Display { get; set; }

        /// <summary>
        ///     An explanation of the meaning of the concept
        /// </summary>
        public string Definition { get; set; }
    }
}
