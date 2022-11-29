using System;

namespace Scrips.Core.Models.Scheduling;

/// <summary>
///     Reminder profile type request
/// </summary>
public class ReminderProfileTypeRequest
{
    /// <summary>
    ///     Id of reminderProfileType
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    ///     Reminder profile id (reference to reminder profile)
    /// </summary>
    public Guid RemiderId { get; set; }

    /// <summary>
    ///     reminder profile type
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    ///     reminider profile time
    /// </summary>
    public string Time { get; set; }

    /// <summary>
    /// </summary>
    public string WhenToSend { get; set; }

    /// <summary>
    ///     reminder profile
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    ///     Custom notes
    /// </summary>
    public string CustomNote { get; set; }
}