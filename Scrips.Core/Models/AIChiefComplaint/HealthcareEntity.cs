using Newtonsoft.Json;

namespace Scrips.Core.Models.AIChiefComplaint;

public class HealthcareEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Text { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public double ConfidenceScore { get; set; }
    public int Offset { get; set; }
    public int Length { get; set; }
    [JsonIgnore]
    public string NormalizedText => Text?.ToLowerInvariant().Trim();
    public ICollection<HealthcareEntityLink> Links { get; set; } = new List<HealthcareEntityLink>();
    public ICollection<HealthcareEntityAttribute> Attributes { get; set; } = new List<HealthcareEntityAttribute>();
    public ICollection<HealthcareEntityRelation> Relations { get; set; } = new List<HealthcareEntityRelation>();
}

