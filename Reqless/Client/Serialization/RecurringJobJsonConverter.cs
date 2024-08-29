using Reqless.Client.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reqless.Client.Serialization;

/// <summary>
/// A custom JSON converter for serializing and deserializing <see
/// cref="RecurringJob"/> instances from the JSON scheme given by server.
/// </summary>
public class RecurringJobJsonConverter : JsonConverter<RecurringJob>
{
    /// <summary>
    /// The properties that are required to be present in the JSON object.
    /// </summary>
    protected static readonly string[] RequiredProperties = {
        "backlog", "count", "data", "interval", "jid", "klass", "priority",
        "queue", "retries", "state", "tags", "throttles",
    };

    /// <summary>
    /// Deserialize the JSON representation of a <see cref="RecurringJob"/>
    /// object into an instance.
    /// </summary>
    /// <param name="reader">The JSON reader to read the object from.</param>
    /// <param name="typeToConvert">The type of object to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override RecurringJob Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Dictionary<string, bool> encounteredProperties = [];
        int? count = null;
        string? data = null;
        int? intervalSeconds = null;
        string? jid = null;
        string? klass = null;
        int? maximumBacklog = null;
        int? priority = null;
        string? queue = null;
        int? retries = null;
        string? state = null;
        string[]? tags = null;
        string[]? throttles = null;

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

            switch (propertyName)
            {
                case "backlog":
                    maximumBacklog = reader.GetInt32();
                    break;
                case "count":
                    count = reader.GetInt32();
                    break;
                case "data":
                    // It is left to job classes to deserialize and validate data.
                    data = BaseJobJsonHelper.ReadData(ref reader);
                    break;
                case "interval":
                    intervalSeconds = reader.GetInt32();
                    break;
                case "jid":
                    jid = BaseJobJsonHelper.ReadJid(ref reader);
                    break;
                case "klass":
                    klass = BaseJobJsonHelper.ReadClassName(ref reader);
                    break;
                case "priority":
                    priority = BaseJobJsonHelper.ReadPriority(ref reader);
                    break;
                case "queue":
                    queue = BaseJobJsonHelper.ReadQueueName(ref reader);
                    break;
                case "retries":
                    retries = BaseJobJsonHelper.ReadRetries(ref reader);
                    break;
                case "state":
                    state = BaseJobJsonHelper.ReadState(ref reader);
                    break;
                case "tags":
                    tags = BaseJobJsonHelper.ReadTags(ref reader);
                    break;
                case "throttles":
                    throttles = BaseJobJsonHelper.ReadThrottles(ref reader);
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

        return new RecurringJob(
            className: klass!,
            count: count!.Value,
            data: data!,
            intervalSeconds: intervalSeconds!.Value,
            jid: jid!,
            maximumBacklog: maximumBacklog!.Value,
            priority: priority!.Value,
            queueName: queue,
            retries: retries!.Value,
            state: state!,
            tags: tags!,
            throttles: throttles!
        );
    }

    /// <summary>
    /// Serialize a <see cref="RecurringJob"/> object into its JSON
    /// representation.
    /// </summary>
    /// <param name="writer">The JSON writer to write the object to.</param>
    /// <param name="value">The Job object to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, RecurringJob value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("backlog", value.MaximumBacklog);
        writer.WriteNumber("count", value.Count);
        BaseJobJsonHelper.WriteData(writer, value.Data);
        writer.WriteNumber("interval", value.IntervalSeconds);
        BaseJobJsonHelper.WriteJid(writer, value.Jid);
        BaseJobJsonHelper.WriteClassName(writer, value.ClassName);
        BaseJobJsonHelper.WritePriority(writer, value.Priority);
        BaseJobJsonHelper.WriteQueueName(writer, value.QueueName);
        BaseJobJsonHelper.WriteRetries(writer, value.Retries);
        BaseJobJsonHelper.WriteState(writer, value.State);
        BaseJobJsonHelper.WriteTags(writer, value.Tags);
        BaseJobJsonHelper.WriteThrottles(writer, value.Throttles);
        writer.WriteEndObject();
    }
}
