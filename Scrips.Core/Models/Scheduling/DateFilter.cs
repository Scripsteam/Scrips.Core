using System.Text.Json.Serialization;

namespace Scrips.Core.Models.Scheduling;

/// <summary>
///     Date Filter.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DateFilter
{
    /// <summary>
    ///     Day
    /// </summary>
    Day,

    /// <summary>
    ///     Week
    /// </summary>
    Week,

    /// <summary>
    ///     Month
    /// </summary>
    Month,

    /// <summary>
    ///     Room
    /// </summary>
    Room,

    /// <summary>
    /// </summary>
    Doctor
}
