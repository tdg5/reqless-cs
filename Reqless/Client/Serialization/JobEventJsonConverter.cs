using Reqless.Client.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reqless.Client.Serialization;

/// <summary>
/// A custom JSON converter for <see cref="JobEvent"/> objects.
/// </summary>
public class JobEventJsonConverter : JsonConverter<JobEvent>
{
    /// <summary>
    /// A dictionary mapping event names to their corresponding types.
    /// </summary>
    protected static Dictionary<string, Type> EventTypesByName { get; } = new()
    {
        { "done", typeof(DoneEvent) },
        { "failed", typeof(FailedEvent) },
        { "failed-retries", typeof(FailedRetriesEvent) },
        { "popped", typeof(PoppedEvent) },
        { "put", typeof(PutEvent) },
        { "throttled", typeof(ThrottledEvent) },
        { "timed-out", typeof(TimedOutEvent) }
    };

    /// <inheritdoc/>
    public override JobEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jobEventType = GetJobEventType(reader);
        var jobEvent = JsonSerializer.Deserialize(ref reader, jobEventType, options);
        return (JobEvent)jobEvent!;
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
    private static Type GetJobEventType(Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected reader to begin with start of object.");
        }

        while (reader.Read())
        {
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
                if (EventTypesByName.TryGetValue(what, out Type? value))
                {
                    return value;
                }
                return typeof(LogEvent);
            }

            reader.Skip();
        }
        throw new JsonException("Expected 'what' property in JSON object, but none was found.");
    }
}