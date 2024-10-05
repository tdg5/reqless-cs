using Reqless.Client.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reqless.Client.Serialization;

/// <summary>
/// A custom JSON converter for <see cref="JidsResult"/> objects.
/// </summary>
public class JidsResultJsonConverter : JsonConverter<JidsResult>
{
    /// <inheritdoc/>
    public override JidsResult Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected reader to begin with start of object.");
        }

        bool jobsEncountered = false;
        bool totalEncountered = false;
        string[]? jids = null;
        int? total = null;

        while (reader.Read())
        {
            // This signifies the end of the job object since nested objects are
            // skipped.
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            // If we're within an object, we expect to start with a property name.
            // We can forgive null here because the reader will throw if the
            // property name isn't a string, so null is impossible here.
            var propertyName = reader.GetString()!;
            reader.Read();

            // Properties other than jobs and total are ignored.
            if (propertyName == "jobs")
            {
                jobsEncountered = true;
                var wasDegenerateObject = JsonConverterHelper.TryConsumeDegenerateObject(
                    "jobs",
                    "array",
                    ref reader
                );
                jids = wasDegenerateObject
                    ? []
                    : JsonSerializer.Deserialize<string[]>(ref reader) ??
                        throw new JsonException(
                            "Failed to deserialize 'jobs' property into a string[]."
                        );
            }
            else if (propertyName == "total")
            {
                totalEncountered = true;
                var isValidTotal = reader.TryGetInt32(out int totalValue);
                if (isValidTotal)
                {
                    total = totalValue;
                }
            }
        }

        if (!jobsEncountered)
        {
            throw new JsonException(
                "Expected 'jobs' property in JSON object, but none was found."
            );
        }

        if (!totalEncountered)
        {
            throw new JsonException(
                "Expected 'total' property in JSON object, but none was found."
            );
        }

        // We can forgive null here parsing values would have thrown or
        // properties would not have been encountered.
        return new JidsResult(jids!, total!.Value);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Write isn't strictly necessary for this converter, but it's included for
    /// completeness. It tries to match the format of the JSON that the server
    /// emits.
    /// </remarks>
    public override void Write(
        Utf8JsonWriter writer,
        JidsResult value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStartObject();
        writer.WriteNumber("total", value.Total);
        writer.WriteStartArray("jobs");
        foreach (var jid in value.Jids)
        {
            writer.WriteStringValue(jid);
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}
