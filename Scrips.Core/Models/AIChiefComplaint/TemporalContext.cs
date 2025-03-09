namespace Scrips.Core.Models.AIChiefComplaint;

public class TemporalContext
{
    public bool IsChronicCondition { get; set; }
    public bool HasAcuteChange { get; set; }
    public string ChangeTimeframe { get; set; }
    public string PriorStatus { get; set; }
}