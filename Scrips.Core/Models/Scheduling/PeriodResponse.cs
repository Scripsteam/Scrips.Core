namespace Scrips.Core.Models.Scheduling;

/// <summary>
///     Time range defined by start and end date/time
///     + Rule: If present, start SHALL have a lower value than end.
/// </summary>
public class PeriodResponse
{
    /// <summary>
    ///     Starting time with inclusive boundary.
    /// </summary>
    public DateTime Start { get; set; }

    /// <summary>
    ///     End time with inclusive boundary, if not ongoing.
    /// </summary>
    public DateTime End { get; set; }
}