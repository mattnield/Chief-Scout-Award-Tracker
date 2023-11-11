namespace OSM.Models;

public class BadgeCompletion
{
    public int ScoutId { get; set; }
    public List<KeyValuePair<string, string>> Achievements { get; set; } = new();
}