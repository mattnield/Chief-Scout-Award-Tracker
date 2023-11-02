using System.Text.Json.Serialization;

namespace OSM.Models;

public class Term
{

    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("termid")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("startdate")] public DateTime StartDate { get; set; }
    [JsonPropertyName("enddate")] public DateTime EndDate { get; set; }

    [JsonIgnore] public bool Current => StartDate <= DateTime.Today && DateTime.Today <= EndDate;

    [JsonIgnore] public bool Past => DateTime.Today > EndDate;
}