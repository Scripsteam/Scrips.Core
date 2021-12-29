using System;

namespace Scrips.Core.Models.Scheduling
{
    public class PeriodResponse
    {
        /// <summary>
        ///     Starting time with inclusive boundary
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        ///     End time with inclusive boundary, if not ongoing
        /// </summary>
        public DateTime End { get; set; }
    }
}
