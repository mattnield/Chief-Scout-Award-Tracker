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
    
    [JsonPropertyName("badges")]
    public BadgeSummary[] Badges { get; set; }
    [JsonPropertyName("patrolid")]
    public int PatrolId { get; set; }
    [JsonPropertyName("patrol_role_level")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int PatrolRoleLevel { get; set; } = 0;
    [JsonPropertyName("patrol_role_level_label")]
    public string PatrolRole { get; set; } = string.Empty;
    [JsonPropertyName("patrol_role_level_abbr")]
    public string PatrolRoleAbbr { get; set; } = string.Empty;
}