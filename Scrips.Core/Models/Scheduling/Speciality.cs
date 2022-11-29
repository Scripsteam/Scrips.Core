using System;
using System.Text.Json.Serialization;

namespace Scrips.Core.Models.Scheduling;

public class Speciality
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("specialitySkillId")]
    public Guid SpecialitySkillId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("code")]
    public PracticeValueSet Code { get; set; }
}