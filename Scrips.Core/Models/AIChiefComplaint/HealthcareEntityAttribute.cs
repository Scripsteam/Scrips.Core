namespace Scrips.Core.Models.AIChiefComplaint;

public class HealthcareEntityAttribute
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Category { get; set; }
    public string Value { get; set; }
    public double ConfidenceScore { get; set; }
}
