namespace Scrips.Core.Models.Scheduling
{
    /// <summary>
    ///     The free/busy status of the slot.
    /// </summary>
    public enum SlotStatusEnum
    {
        /// <summary>
        ///     Indicates that the time interval is busy because one or more events have been scheduled for that interval.
        /// </summary>
        Busy,

        /// <summary>
        ///     Indicates that the time interval is free for scheduling.
        /// </summary>
        Free,

        /// <summary>
        ///     Indicates that the time interval is busy and that the interval cannot be scheduled.
        /// </summary>
        BusyUnavailable,

        /// <summary>
        ///     Indicates that the time interval is busy because one or more events have been tentatively scheduled for that
        ///     interval.
        /// </summary>
        BusyTentative,

        /// <summary>
        ///     This instance should not have been part of this patient's medical record.
        /// </summary>
        EnteredInError
    }
}
