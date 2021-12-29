namespace Scrips.Core.Models.Scheduling
{
    public class ParticipationType
    {
        /// <summary>
        ///     Code defined by a terminology system
        /// </summary>
        public ParticipationTypeEnum Code { get; set; }

        /// <summary>
        ///     Definition
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        ///     A human language representation of the concept as seen/selected/uttered by the user who entered the data
        ///     and/or which represents the intended meaning of the user.
        /// </summary>
        public string Display { get; set; }

        /// <summary>
        ///     System
        /// </summary>
        public string System { get; set; }
    }
}
