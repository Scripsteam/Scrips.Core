using System;

namespace Scrips.Core.Models.Scheduling;

/// <summary>
///     Request to get available slots based on the filters to book an appointment.
/// </summary>
public class SlotsRequest
{
    /// <summary>
    ///     Id of the Practice.
    /// </summary>

    public Guid? PracticeId { get; set; }

    /// <summary>
    ///     Id of the Practitioner.
    /// </summary>
    public Guid PractitionerId { get; set; }

    /// <summary>
    ///     Start date time needs to be > current date time
    ///     All available slots >= Start date will be returned if start date provided, else available slots >= current date
    ///     time will be returned.
    /// </summary>
    public DateTime? Start { get; set; }

    /// <summary>
    ///     End date time
    ///     All available slots = End date will be returned if end date provided, else available slots provided based on the
    ///     count.
    /// </summary>
    public DateTime? End { get; set; }

    /// <summary>
    ///     Appointment profile id.
    /// </summary>

    public Guid? AppointmentProfileId { get; set; }

    /// <summary>
    ///     Count of available slots needed.
    ///     Default is 5.
    /// </summary>
    public int Count { get; set; } = 5;

    /// <summary>
    ///     SearchFromUserApp is uses when slots are search for all location
    /// </summary>
    public bool SearchFromUserApp { get; set; } = false;

}