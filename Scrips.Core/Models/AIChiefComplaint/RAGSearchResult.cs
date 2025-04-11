namespace Scrips.Core.Models.AIChiefComplaint;

public class RAGSearchResult
{
    public IReadOnlyList<ChiefComplaintDocument> SimilarComplaints { get; set; }
    public AnalyzedComplaint ClinicalContext { get; set; }
    public IList<string> TemplateRecommendations { get; set; }
    public IList<string> DocumentationGuidelines { get; set; }
}
