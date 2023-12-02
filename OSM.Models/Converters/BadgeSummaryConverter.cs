using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OSM.Models.Converters;

public class BadgeSummaryConverter : JsonConverter<BadgeSummary>
{
    public override BadgeSummary Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var badgeSummary = new BadgeSummary();
        List<KeyValuePair<string, string>> valuesList = new List<KeyValuePair<string, string>>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read(); // Move to the property value

                try
                {
                    if ((new[] {"a", "b"}).Contains(propertyName))
                    {
                        switch (reader.TokenType)
                        {
                            case JsonTokenType.Number:
                                valuesList.Add(
                                    new KeyValuePair<string, string>(propertyName.TrimStart('_'),
                                        reader.GetInt32().ToString() ?? string.Empty));
                                break;
                            default:
                                valuesList.Add(
                                    new KeyValuePair<string, string>(propertyName.TrimStart('_'),
                                        reader.GetString() ?? string.Empty));
                                break;
                        }
                    }
                    else
                    {
                        // Handle other properties of the Scout class
                        switch (propertyName)
                        {
                            case "awarded_date":
                                if (reader.TokenType == JsonTokenType.Number)
                                {
                                    var longValue = reader.GetInt64();
                                    if (longValue > 0)
                                        badgeSummary.AwardedDate =
                                            DateTimeOffset.FromUnixTimeSeconds(longValue).DateTime;
                                }
                                else
                                {
                                    var awardedDateValue = reader.GetString();
                                    if (DateTime.TryParseExact(awardedDateValue, "yyyy'-'MM'-'dd",
                                            CultureInfo.InvariantCulture,
                                            DateTimeStyles.None,
                                            out var date))
                                    {
                                        badgeSummary.AwardedDate = date;
                                    }
                                }

                                break;
                            case "level":
                                if (reader.TokenType == JsonTokenType.False)
                                {
                                    badgeSummary.LevelText = string.Empty;
                                    reader.GetBoolean();
                                }
                                else
                                {
                                    badgeSummary.LevelText = reader.GetString();
                                }

                                break;
                            case "badge_group":
                                badgeSummary.BadgeType = (BadgeType) Enum.Parse(typeof(BadgeType), reader.GetString());
                                break;
                            case "badge_id":
                                badgeSummary.Id = int.Parse(reader.GetString());
                                break;
                            case "badge":
                                badgeSummary.Name = reader.GetString();
                                break;
                            case "badge_identifier":
                                badgeSummary.BadgeVersionId = reader.GetString();
                                break;
                            case "picture":
                                badgeSummary.Image = reader.GetString();
                                break;
                            case "completed":
                                badgeSummary.CompletedLevel = int.Parse(reader.GetString());
                                break;
                            default:
                                reader.Skip();
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine($"Fail at {propertyName}");
                }
            }
        }

        return badgeSummary;
    }

    public override void Write(Utf8JsonWriter writer, BadgeSummary badgeSummary, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteEndObject();
    }
}