using Newtonsoft.Json;

namespace Scrips.Core.Models.Patient;

public class RelationResponse
{
    [JsonProperty("Id")]
    public Guid Id { get; set; }
    [JsonProperty("Name")]
    public string Name { get; set; }
}