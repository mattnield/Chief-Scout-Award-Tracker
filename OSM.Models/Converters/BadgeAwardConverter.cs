using System.Text.Json;
using System.Text.Json.Serialization;

namespace OSM.Models.Converters;

public class BadgeAwardConverter : JsonConverter<BadgeCompletion>
{
    public override BadgeCompletion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var badgeCompletion = new BadgeCompletion();
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
                    if (propertyName.StartsWith("_") && double.TryParse(propertyName[1..], out _))
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
                            case "scoutid":
                                badgeCompletion.ScoutId = reader.GetInt32();
                                break;
                            case "awarded":
                                badgeCompletion.Awarded = (reader.GetString() == "1");
                                break;
                            case "completed":
                                badgeCompletion.Completed = (reader.GetString() == "1");
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

        badgeCompletion.Achievements = valuesList;
        return badgeCompletion;
    }

    public override void Write(Utf8JsonWriter writer, BadgeCompletion badgeCompletion, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("scoutid", badgeCompletion.ScoutId);

        foreach (var pair in badgeCompletion.Achievements)
        {
            writer.WriteString($"_{pair.Key}", pair.Value);
        }

        writer.WriteEndObject();
    }
}