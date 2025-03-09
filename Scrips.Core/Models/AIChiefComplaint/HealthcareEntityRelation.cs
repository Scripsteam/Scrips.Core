namespace Scrips.Core.Models.AIChiefComplaint;

public class HealthcareEntityRelation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RelationType { get; set; }
    public string TargetEntityId { get; set; }
    public double ConfidenceScore { get; set; }
}
