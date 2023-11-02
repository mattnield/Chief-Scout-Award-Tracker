using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace OSM.Models;

public class Badge
{
    [JsonPropertyName("badge_id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("badge_version")] public string Version { get; set; } = string.Empty;
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("parents_description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("type_id")] public BadgeType Type { get; set; }

    [JsonIgnore] public Dictionary<string, List<Criteria>> Areas { get; set; } = new();
    
    [JsonIgnore] public List<Criteria> Criteria { get; set; } = new();
    [JsonIgnore] public Config Config { get; set; } = new();

    /*
    The `config` element contains more information as JSON.

    For the Skill challenge badge, we can see `{\"requires\":[[\"a\"],[\"b\"],[\"c\"]]}`. This just tells us that we
    need something from all three areas. What we can't see is that area 'b' only needs 5 items to be completed. This is
    probably stored somewhere - but it's not clear where.

    For the Outdoors challenge badge, we can see
    `{\"requires\":[[\"a\"],[\"b\"]],\"columnsRequired\":[{\"min\":8,\"id\":\"21037\"}]}`. Again, this tells us that
    there are two areas. It also tells us that criteria 21037 needs to have a minimum value of 8. This criteria is
    from the Nights Away staged activity badge. This means that when we render this badge in a table, we should
    try to pull through tha value and badge also.
    */

    [JsonPropertyName("badge_order")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Order { get; set; }
}

public record Criteria
{
    [JsonPropertyName("field")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("tooltip")] public string Description { get; set; } = string.Empty;
    [JsonPropertyName("module")] public string Module { get; set; } = string.Empty;
}

public record Config
{
    [JsonPropertyName("requires")] public List<string> Requires { get; set; } = new();
    [JsonPropertyName("columnRequired")] public List<ColumnRequired> ColumnsRequired { get; set; } = new();
}

public record ColumnRequired
{
    [JsonPropertyName("min")] public long Min { get; set; }
    [JsonPropertyName("id")] public string CriteriaId { get; set; } = string.Empty;
}

[JsonConverter(typeof(StringConverter))]
public enum BadgeType
{
    [EnumMember(Value = "1")] Challenge = 1,
    [EnumMember(Value = "2")] Activity = 2,
    [EnumMember(Value = "3")] Staged = 3,
    [EnumMember(Value = "4")] Core = 4,
}