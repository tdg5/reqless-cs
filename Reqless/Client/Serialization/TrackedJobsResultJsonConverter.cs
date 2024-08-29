using Reqless.Client.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reqless.Client.Serialization;

/// <summary>
/// A custom JSON converter for <see cref="TrackedJobsResult"/> objects.
/// </summary>
public class TrackedJobsResultJsonConverter : JsonConverter<TrackedJobsResult>
{
    /// <inheritdoc/>
    public override TrackedJobsResult Read(
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
        bool expiredEncountered = false;
        Job[]? jobs = null;
        string[]? expiredJids = null;

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
                jobs = wasDegenerateObject
                    ? []
                    : JsonSerializer.Deserialize<Job[]>(ref reader) ??
                        throw new JsonException(
                            "Failed to deserialize 'jobs' property into a Job[]."
                        );
            }
            else if (propertyName == "expired")
            {
                expiredEncountered = true;
                var wasDegenerateObject = JsonConverterHelper.TryConsumeDegenerateObject(
                    "expired",
                    "array",
                    ref reader
                );
                expiredJids = wasDegenerateObject
                    ? []
                    : JsonSerializer.Deserialize<string[]>(ref reader) ??
                        throw new JsonException(
                            "Failed to deserialize 'expired' property into a string[]."
                        );
            }
        }

        if (!jobsEncountered)
        {
            throw new JsonException(
                "Expected 'jobs' property in JSON object, but none was found."
            );
        }

        if (!expiredEncountered)
        {
            throw new JsonException(
                "Expected 'expired' property in JSON object, but none was found."
            );
        }

        return new TrackedJobsResult(jobs: jobs!, expiredJids: expiredJids!);
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        TrackedJobsResult value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStartObject();
        writer.WriteStartArray("jobs");
        foreach (var job in value.Jobs)
        {
            JsonSerializer.Serialize(writer, job, options);
        }
        writer.WriteEndArray();
        writer.WriteStartArray("expired");
        foreach (var expired in value.ExpiredJids)
        {
            JsonSerializer.Serialize(writer, expired, options);
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}