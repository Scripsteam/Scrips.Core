namespace Scrips.Core.Models.Practice;

/// <summary>
/// Response for slots.
/// </summary>
public class SlotResponse
{
    /// <summary>
    /// System defined id.
    /// </summary>
    public Guid Id { get; set; }


    /// <summary>
    /// The style of appointment or patient that may be booked in the slot (not service type)
    /// </summary>
    public AppointmentType AppointmentType { get; set; }

    /// <summary>
    /// The schedule resource that this slot defines an interval of status information
    /// </summary>
    public Guid ScheduleId { get; set; }

    /// <summary>
    /// The free/busy status of the slot.
    /// </summary>
    public SlotStatus Status { get; set; }

    /// <summary>
    /// Date/Time that the slot is to begin
    /// </summary>
    public DateTime Start { get; set; }

    /// <summary>
    /// Date/Time that the slot is to conclude
    /// </summary>
    public DateTime End { get; set; }

    /// <summary>
    /// Comments on the slot to describe any extended information. Such as custom constraints on the slot
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// This slot has already been overbooked, appointments are unlikely to be accepted for this time
    /// </summary>
    public bool Overbooked { get; set; }

    public Guid PracticeId { get; set; }

    public string PracticeName { get; set; }
}

/// <summary>
/// The coded reason that this appointment is being scheduled. This is more clinical than administrative.
/// </summary>
public class SlotStatus
{
    /// <summary>
    /// Code defined by a terminology system
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// An explanation of the meaning of the concept
    /// </summary>
    public string Definition { get; set; }

    /// <summary>
    /// A human language representation of the concept as seen/selected/uttered by the user who entered the data
    /// and/or which represents the intended meaning of the user.
    /// </summary>
    // [Required]
    public string Display { get; set; }
}

public class SlotResponseList
{
    public SlotResponseList()
    {
        SlotResponses = new List<SlotResponse>();
    }

    public IList<SlotResponse> SlotResponses { get; set; }

    public int SlotCount { get; set; }
}