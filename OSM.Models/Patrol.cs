using System.Text.Json.Serialization;

namespace OSM.Models;

public class Patrol
{
    [JsonPropertyName("patrolid")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("members")]
    public List<Member> Members { get; set; } = new();
}