namespace Scrips.Core.Models.AIChiefComplaint;

public class HealthcareEntityLink
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DataSource { get; set; }
    public string Code { get; set; }
    public List<string> Synonyms { get; set; } = new List<string>();
}
