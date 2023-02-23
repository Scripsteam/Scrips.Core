using System.Text.Json.Serialization;

namespace Scrips.Core.Enums.Scheduling;

/// <summary>
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum QuestionnaireStatus
{
    /// <summary>
    ///     Office.
    /// </summary>
    Send,

    /// <summary>
    ///     Home.
    /// </summary>
    Pending,

    /// <summary>
    /// Answered.
    /// </summary>
    Answered
}