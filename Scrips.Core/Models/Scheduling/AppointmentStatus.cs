namespace Scrips.Core.Models.Scheduling;

/// <summary>
///     The overall status of the Appointment.
///     Each of the participants has their own participation status which indicates their involvement in the process,
///     however this status indicates the shared status.
/// </summary>
public class AppointmentStatus
{
    /// <summary>
    ///     Code defined by a terminology system
    /// </summary>
    public Enums.Scheduling.AppointmentStatus Code { get; set; }

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