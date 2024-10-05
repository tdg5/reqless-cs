using Reqless.Client.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reqless.Client.Serialization;

/// <summary>
/// A custom JSON converter for <see cref="LogEvent"/> objects.
/// </summary>
public class LogEventJsonConverter : JsonConverter<LogEvent>
{
    /// <inheritdoc/>
    public override LogEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected reader to begin with start of object.");
        }

        bool whatEncountered = false;
        string? what = null;
        bool whenEncountered = false;
        long? when = null;
        Dictionary<string, JsonElement>? data = [];

        while (reader.Read())
        {
            // This signifies the end of the job object since nested objects are handled separately.
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            // If we're within an object, we expect to start with a property name.
            // We can forgive null here because the reader will throw if the
            // property name isn't a string, so null is impossible here.
            var propertyName = reader.GetString()!;

            // Now move on to the property value.
            reader.Read();
            if (propertyName == "what")
            {
                whatEncountered = true;
                what = reader.GetString();
                continue;
            }
            else if (propertyName == "when")
            {
                whenEncountered = true;
                var isValidWhen = reader.TryGetInt64(out long whenValue);
                if (isValidWhen)
                {
                    when = whenValue;
                }
            }
            else
            {
                data[propertyName] = JsonElement.ParseValue(ref reader);
            }
        }

        if (!whatEncountered)
        {
            throw new JsonException(
                "Expected 'what' property in JSON object, but none was found."
            );
        }

        if (!whenEncountered)
        {
            throw new JsonException(
                "Expected 'when' property in JSON object, but none was found."
            );
        }
        if (what is null)
        {
            throw new JsonException(
                "Expected a string value for the 'what' property, got null."
            );
        }
        // We can forgive null here for when because an error would have
        // occurred if when were omitted or invalid.
        return new LogEvent(what, when!.Value, data);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, LogEvent value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("what", value.What);
        writer.WriteNumber("when", value.When);
        foreach (var (key, dataValue) in value.Data)
        {
            writer.WritePropertyName(key);
            dataValue.WriteTo(writer);
        }
        writer.WriteEndObject();
    }
}
