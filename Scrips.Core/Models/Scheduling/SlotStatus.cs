namespace Scrips.Core.Models.Scheduling
{
    public class SlotStatus
    {
        /// <summary>
        ///     Code defined by a terminology system
        /// </summary>
        public Enums.Scheduling.SlotStatus Code { get; set; }

        /// <summary>
        ///     An explanation of the meaning of the concept
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        ///     A human language representation of the concept as seen/selected/uttered by the user who entered the data
        ///     and/or which represents the intended meaning of the user.
        /// </summary>
        public string Display { get; set; }
    }
}
