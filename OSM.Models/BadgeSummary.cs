using System.Text.Json.Serialization;
using OSM.Models.Converters;

namespace OSM.Models;

[JsonConverter(typeof(BadgeSummaryConverter))]
public class BadgeSummary
{
    public int Id { get; set; }
    public int CompletedLevel { get; set; }
    public bool Awarded => AwardedDate.HasValue;
    public DateTime? AwardedDate { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName => string.Join(' ', Name, LevelText).Trim();
    public BadgeType BadgeType { get; set; }
    public bool IsComplete => CompletedLevel > 0;
    public string Image { get; set; } = string.Empty;
    public string BadgeVersionId { get; set; } = string.Empty;
    public string LevelText { get; set; } = string.Empty;
}