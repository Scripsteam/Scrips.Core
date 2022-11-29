using System;

namespace Scrips.Core.Models.Scheduling;

/// <summary>
///     Response for slots.
/// </summary>
public class SlotResponse
{
    /// <summary>
    ///     System defined id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    ///     The style of appointment or patient that may be booked in the slot (not service type)
    /// </summary>
    public AppointmentType AppointmentType { get; init; }

    /// <summary>
    ///     The schedule resource that this slot defines an interval of status information
    /// </summary>
    public Guid ScheduleId { get; init; }

    /// <summary>
    ///     The free/busy status of the slot.
    /// </summary>
    public SlotStatus Status { get; init; }

    /// <summary>
    ///     Date/Time that the slot is to begin
    /// </summary>
    public DateTime Start { get; set; }

    /// <summary>
    ///     Date/Time that the slot is to conclude
    /// </summary>
    public DateTime End { get; set; }

    /// <summary>
    ///     Comments on the slot to describe any extended information. Such as custom constraints on the slot
    /// </summary>
    public string Comment { get; init; }

    /// <summary>
    ///     This slot has already been overbooked, appointments are unlikely to be accepted for this time
    /// </summary>
    public bool Overbooked { get; init; }

    /// <summary>
    /// ID of Practice
    /// </summary>
    public Guid PracticeId { get; init; }

    /// <summary>
    /// Name of Practice
    /// </summary>
    public string PracticeName { get; init; }

}