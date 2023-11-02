using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OSM.JsonConverters;

public class EnumStringConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {if (reader.TokenType == JsonTokenType.String)
        {
            string enumValueString = reader.GetString();

            foreach (var field in typeToConvert.GetFields())
            {
                var enumMemberAttribute = (EnumMemberAttribute)Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute));

                if (enumMemberAttribute != null && enumMemberAttribute.Value == enumValueString)
                {
                    return (TEnum)field.GetValue(null);
                }
            }
        }

        throw new JsonException($"Unable to parse {typeToConvert.Name} from the provided JSON.");
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        foreach (var field in typeof(TEnum).GetFields())
        {
            var enumMemberAttribute = (EnumMemberAttribute)Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute));

            if (enumMemberAttribute != null && ((TEnum) field.GetValue(null)).Equals(value))
            {
                writer.WriteStringValue(enumMemberAttribute.Value);
                return;
            }
        }

        throw new JsonException($"Unable to serialize {typeof(TEnum).Name} to JSON.");
    }
}