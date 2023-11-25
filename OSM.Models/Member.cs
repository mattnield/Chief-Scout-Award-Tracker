using System.Text.Json.Serialization;

namespace OSM.Models;

public class Member
{
    [JsonPropertyName("scoutid")]
    public int Id { get; set; }
    [JsonPropertyName("firstname")]
    public string FirstName { get; set; } = string.Empty;
    [JsonIgnore]
    public string Initial => LastName.Substring(0, 1) ?? string.Empty;
    [JsonPropertyName("lastname")]
    public string LastName { get; set; } = string.Empty;
    [JsonPropertyName("patrolid")]
    public int PatrolId { get; set; }
    [JsonPropertyName("badges")]
    public BadgeSummary[] Badges { get; set; }
}