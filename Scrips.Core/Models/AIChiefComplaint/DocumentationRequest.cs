namespace Scrips.Core.Models.AIChiefComplaint;

public class DocumentationRequest
{
    public Guid ComplaintId { get; set; }
    public Dictionary<string, List<string>> SelectedElements { get; set; }
    public string SpecialtyContext { get; set; }
}
