using Reqless.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reqless.Serialization;

/// <summary>
/// A custom JSON converter for <see cref="WorkerJobs"/> objects.
/// </summary>
public class WorkerJobsJsonConverter : JsonConverter<WorkerJobs>
{
    /// <inheritdoc/>
    public override WorkerJobs Read(
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
        bool stalledEncountered = false;
        string[]? jobs = null;
        string[]? stalledJids = null;

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
                    : JsonSerializer.Deserialize<string[]>(ref reader) ??
                        throw new JsonException(
                            "Failed to deserialize 'jobs' property into a string[]."
                        );
            }
            else if (propertyName == "stalled")
            {
                stalledEncountered = true;
                var wasDegenerateObject = JsonConverterHelper.TryConsumeDegenerateObject(
                    "stalled",
                    "array",
                    ref reader
                );
                stalledJids = wasDegenerateObject
                    ? []
                    : JsonSerializer.Deserialize<string[]>(ref reader) ??
                        throw new JsonException(
                            "Failed to deserialize 'stalled' property into a string[]."
                        );
            }
        }

        if (!jobsEncountered)
        {
            throw new JsonException(
                "Expected 'jobs' property in JSON object, but none was found."
            );
        }

        if (!stalledEncountered)
        {
            throw new JsonException(
                "Expected 'stalled' property in JSON object, but none was found."
            );
        }

        return new WorkerJobs(jobs: jobs!, stalled: stalledJids!);
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        WorkerJobs value,
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
        writer.WriteStartArray("stalled");
        foreach (var stalled in value.Stalled)
        {
            JsonSerializer.Serialize(writer, stalled, options);
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}