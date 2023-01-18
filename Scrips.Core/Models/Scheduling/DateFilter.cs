using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
