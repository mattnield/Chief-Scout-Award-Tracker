using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OSM.Models.Converters;

public class BadgeConverter : JsonConverter<Badge>
{
    public override Badge Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var badge = new Badge();
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
                            case "type_id":
                                badge.BadgeType = (BadgeType) Enum.Parse(typeof(BadgeType), reader.GetString());
                                break;
                            case "badge_id":
                                badge.Id = int.Parse(reader.GetString());
                                break;
                            case "badge_version":
                                badge.Version = reader.GetString();
                                break;
                            case "badge":
                                badge.Name = reader.GetString();
                                break;
                            case "badge_identifier":
                                badge.BadgeIdentifier = reader.GetString();
                                break;
                            case "picture":
                                badge.Image = reader.GetString();
                                break;
                            case "name":
                                badge.Name = reader.GetString();
                                break;
                            case "description":
                                badge.Description = reader.GetString();
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

        return badge;
    }

    public override void Write(Utf8JsonWriter writer, Badge badge, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteEndObject();
    }
}