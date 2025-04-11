namespace Scrips.Core.Models.AIChiefComplaint;

public class HealthcareAnalysis
{
    public IEnumerable<HealthcareEntity> Symptoms { get; set; }
    public IEnumerable<HealthcareEntity> Conditions { get; set; }
    public IEnumerable<HealthcareEntity> AnatomicalSites { get; set; }
    public string Severity { get; set; }
    public string Duration { get; set; }
    public TemporalContext TemporalContext { get; set; }
}
