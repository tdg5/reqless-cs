using Reqless.Models.JobEvents;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reqless.Serialization;

/// <summary>
/// A custom JSON converter for <see cref="JobEvent"/> objects.
/// </summary>
public class JobEventJsonConverter : JsonConverter<JobEvent>
{
    /// <inheritdoc/>
    public override JobEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jobEventType = GetJobEventType(reader);

        JobEvent jobEvent = jobEventType switch
        {
            "done" => JsonSerializer.Deserialize<DoneEvent>(ref reader, options)!,
            "failed" => JsonSerializer.Deserialize<FailedEvent>(ref reader, options)!,
            "failed-retries" => JsonSerializer.Deserialize<FailedRetriesEvent>(ref reader, options)!,
            "popped" => JsonSerializer.Deserialize<PoppedEvent>(ref reader, options)!,
            "put" => JsonSerializer.Deserialize<PutEvent>(ref reader, options)!,
            "throttled" => JsonSerializer.Deserialize<ThrottledEvent>(ref reader, options)!,
            "timed-out" => JsonSerializer.Deserialize<TimedOutEvent>(ref reader, options)!,
            _ => throw new JsonException($"Unknown job event type: {jobEventType}.")
        };
        return jobEvent!;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, JobEvent value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }

    /// <summary>
    /// Gets the job event type from the JSON reader by examining the "what"
    /// property.
    /// </summary>
    /// <param name="reader">The JSON reader to read from.</param>
    private static string GetJobEventType(Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected reader to begin with start of object.");
        }
        var currentDepth = 0;
        while (reader.Read())
        {
            if (
                reader.TokenType == JsonTokenType.StartObject
                || reader.TokenType == JsonTokenType.StartArray
            )
            {
                currentDepth++;
                continue;
            }

            if (
                reader.TokenType == JsonTokenType.EndObject
                || reader.TokenType == JsonTokenType.EndArray
            )
            {
                currentDepth--;
                continue;
            }

            if (currentDepth > 0)
            {
                continue;
            }

            if (
                reader.TokenType == JsonTokenType.PropertyName
                && reader.GetString() == "what"
            )
            {
                reader.Read();
                var what = reader.GetString()
                    ?? throw new JsonException(
                        "Expected a string value for the 'what' property, got null."
                    );
                return what;
            }
        }
        throw new JsonException("Expected 'what' property in JSON object, but none was found.");
    }
}