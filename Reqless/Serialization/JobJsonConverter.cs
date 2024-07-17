using Reqless.Models;
using Reqless.Models.JobEvents;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reqless.Serialization;

/// <summary>
/// A custom JSON converter for serializing and deserializing <see cref="Job"/>
/// instances from the JSON scheme given by server.
/// </summary>
public class JobJsonConverter : JsonConverter<Job>
{
    /// <summary>
    /// The properties that are required to be present in the JSON object.
    /// </summary>
    protected static readonly string[] RequiredProperties = {
        "data", "dependencies", "dependents", "expires", "failure",
        "history", "jid", "klass", "priority", "queue", "remaining",
        "retries", "spawned_from_jid", "state", "tags", "throttles",
        "tracked", "worker",
    };

    /// <summary>
    /// Deserialize the JSON representation of a <see cref="Job"/> object into
    /// an instance.
    /// </summary>
    /// <param name="reader">The JSON reader to read the object from.</param>
    /// <param name="typeToConvert">The type of object to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override Job Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Dictionary<string, bool> encounteredProperties = [];
        string? data = null;
        string[]? dependencies = null;
        string[]? dependents = null;
        long? expires = null;
        JobFailure? failure = null;
        JobEvent[]? history = null;
        string? jid = null;
        string? klass = null;
        int? priority = null;
        string? queue = null;
        int? remaining = null;
        int? retries = null;
        string? spawnedFromJid = null;
        string? state = null;
        string[]? tags = null;
        string[]? throttles = null;
        bool? tracked = null;
        string? worker = null;

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
            encounteredProperties[propertyName] = true;

            // Now move on to the property value.
            reader.Read();

            bool wasDegenerateObject;

            switch (propertyName)
            {
                case "data":
                    // It is left to job classes to deserialize and validate data.
                    data = reader.GetString();
                    break;
                case "dependencies":
                    wasDegenerateObject = JsonConverterHelper.TryConsumeDegenerateObject(
                        "dependencies",
                        "array",
                        ref reader
                    );
                    dependencies = wasDegenerateObject
                        ? []
                        : JsonSerializer.Deserialize<string[]>(ref reader);
                    break;
                case "dependents":
                    wasDegenerateObject = JsonConverterHelper.TryConsumeDegenerateObject(
                        "dependents",
                        "array",
                        ref reader
                    );
                    dependents = wasDegenerateObject
                        ? []
                        : JsonSerializer.Deserialize<string[]>(ref reader);
                    break;
                case "expires":
                    reader.TryGetInt64(out var expiresValue);
                    if (expiresValue > 0)
                    {
                        expires = expiresValue;
                    }
                    break;
                case "failure":
                    var readerClone = reader;
                    readerClone.Read();
                    var isEmptyObject = (
                        reader.TokenType == JsonTokenType.StartObject &&
                        readerClone.TokenType == JsonTokenType.EndObject
                    );
                    if (isEmptyObject)
                    {
                        failure = null;
                        reader.Read();
                    }
                    else
                    {
                        failure = JsonSerializer.Deserialize<JobFailure>(ref reader);
                    }
                    break;
                case "history":
                    wasDegenerateObject = JsonConverterHelper.TryConsumeDegenerateObject(
                        "history",
                        "array",
                        ref reader
                    );
                    history = wasDegenerateObject
                        ? []
                        : JsonSerializer.Deserialize<JobEvent[]>(ref reader);
                    break;
                case "jid":
                    jid = reader.GetString();
                    break;
                case "klass":
                    klass = reader.GetString();
                    break;
                case "priority":
                    priority = reader.GetInt32();
                    break;
                case "queue":
                    queue = reader.GetString();
                    break;
                case "remaining":
                    remaining = reader.GetInt32();
                    break;
                case "retries":
                    retries = reader.GetInt32();
                    break;
                case "spawned_from_jid":
                    if (reader.TokenType == JsonTokenType.False)
                    {
                        spawnedFromJid = null;
                    }
                    else
                    {
                        spawnedFromJid = reader.GetString();
                    }
                    break;
                case "state":
                    state = reader.GetString();
                    break;
                case "tags":
                    wasDegenerateObject = JsonConverterHelper.TryConsumeDegenerateObject(
                        "tags",
                        "array",
                        ref reader
                    );
                    tags = wasDegenerateObject
                        ? []
                        : JsonSerializer.Deserialize<string[]>(ref reader);
                    break;
                case "throttles":
                    wasDegenerateObject = JsonConverterHelper.TryConsumeDegenerateObject(
                        "throttles",
                        "array",
                        ref reader
                    );
                    throttles = wasDegenerateObject
                        ? []
                        : JsonSerializer.Deserialize<string[]>(ref reader);
                    break;
                case "tracked":
                    tracked = reader.GetBoolean();
                    break;
                case "worker":
                    worker = reader.GetString();
                    if (string.IsNullOrWhiteSpace(worker))
                    {
                        worker = null;
                    }
                    break;
                default:
                    Console.WriteLine($"Unknown property: {propertyName}");
                    reader.Skip();
                    break;
            }
        }

        foreach (var requiredProperty in RequiredProperties)
        {
            if (!encounteredProperties.ContainsKey(requiredProperty))
            {
                throw new JsonException($"Required property '{requiredProperty}' not found.");
            }
        }

        return new Job(
            className: klass!,
            data: data!,
            dependencies: dependencies!,
            dependents: dependents!,
            expires: expires,
            failure: failure,
            history: history!,
            jid: jid!,
            priority: priority!.Value,
            queueName: queue!,
            remaining: remaining!.Value,
            retries: retries!.Value,
            spawnedFromJid: spawnedFromJid,
            state: state!,
            tags: tags!,
            throttles: throttles!,
            tracked: tracked!.Value,
            workerName: worker!
        );
    }

    /// <summary>
    /// Serialize a <see cref="Job"/> object into its JSON representation.
    /// </summary>
    /// <param name="writer">The JSON writer to write the object to.</param>
    /// <param name="value">The Job object to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, Job value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("data", value.Data);
        WriteStringArray(writer, "dependencies", value.Dependencies);
        WriteStringArray(writer, "dependents", value.Dependents);
        writer.WriteNumber("expires", value.Expires ?? 0);
        WriteObject(writer, "failure", value.Failure);
        WriteHistoryArray(writer, "history", value.History);
        writer.WriteString("jid", value.Jid);
        writer.WriteString("klass", value.ClassName);
        writer.WriteNumber("priority", value.Priority);
        writer.WriteString("queue", value.QueueName);
        writer.WriteNumber("remaining", value.Remaining);
        writer.WriteNumber("retries", value.Retries);
        writer.WriteString("spawned_from_jid", value.SpawnedFromJid);
        writer.WriteString("state", value.State);
        WriteStringArray(writer, "tags", value.Tags);
        WriteStringArray(writer, "throttles", value.Throttles);
        writer.WriteBoolean("tracked", value.Tracked);
        writer.WriteString("worker", value.WorkerName);
        writer.WriteEndObject();
    }

    private static void WriteHistoryArray(Utf8JsonWriter writer, string propertyName, JobEvent[] values)
    {
        writer.WriteStartArray(propertyName);
        foreach (var value in values)
        {
            var rawEventJson = JsonSerializer.Serialize(value);
            writer.WriteRawValue(rawEventJson);
        }
        writer.WriteEndArray();
    }

    private static void WriteObject(Utf8JsonWriter writer, string propertyName, JobFailure? value)
    {
        if (value is null)
        {
            writer.WriteNull(propertyName);
            return;
        }

        var objectJson = JsonSerializer.Serialize(value);
        writer.WritePropertyName(propertyName);
        writer.WriteRawValue(objectJson);
    }

    private static void WriteStringArray(Utf8JsonWriter writer, string propertyName, string[] values)
    {
        writer.WritePropertyName(propertyName);
        writer.WriteStartArray();
        foreach (var value in values)
        {
            writer.WriteStringValue(value);
        }
        writer.WriteEndArray();
    }
}
