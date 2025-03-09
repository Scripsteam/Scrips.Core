using Azure.Search.Documents.Indexes;
using System.Text.Json.Serialization;

namespace Scrips.Core.Models.AIChiefComplaint;

public class ChiefComplaintDocument
{
    [SimpleField(IsKey = true, IsFilterable = false)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [SearchableField(
        AnalyzerName = "standard.lucene",
        IsSortable = true,
        IsFilterable = true)]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [SearchableField(
        IsFilterable = true,
        IsFacetable = true,
        AnalyzerName = "standard.lucene")]
    [JsonPropertyName("categories")]
    public IList<string> Categories { get; set; } = new List<string>();

    [SearchableField(
        IsFilterable = true,
        AnalyzerName = "standard.lucene")]
    [JsonPropertyName("icd10Codes")]
    public IList<string> ICD10Codes { get; set; } = new List<string>();

    [VectorSearchField(
        VectorSearchDimensions = 1536,
        VectorSearchProfileName = "cc-vector-profile")]
    [JsonPropertyName("embedding")]
    public float[] Embedding { get; set; }
}
