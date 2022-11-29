namespace Scrips.Core.Models.Scheduling;

/// <summary>
///     The coded reason for the appointment being cancelled.
///     This is often used in reporting/billing/further processing to determine if further actions are required,
///     or specific fees apply.
/// </summary>
public class AppointmentCancellationReason
{
    /// <summary>
    ///     Code defined by a terminology system
    /// </summary>
    public Enums.Scheduling.AppointmentCancellationReason Code { get; set; }

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