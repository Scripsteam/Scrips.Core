using System.Text.Json.Serialization;

namespace Scrips.Core.Models.Scheduling;

/// <summary>
/// 
/// </summary>
public class PracticeValueSet
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string System { get; set; } = "http://snomed.info/sct";
}