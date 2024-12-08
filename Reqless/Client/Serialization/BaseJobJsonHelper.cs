using Reqless.Client.Models;
using System.Text.Json;

namespace Reqless.Client.Serialization;

/// <summary>
/// Helper class for serializing and deserializing <see cref="BaseJob"/>
/// properties.
/// </summary>
public static class BaseJobJsonHelper
{
    /// <summary>
    /// Read the klass property from the JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> containing job
    /// data.</param>
    /// <returns>The class name of the job or null.</returns>
    public static string? ReadClassName(ref Utf8JsonReader reader)
    {
        return reader.GetString();
    }

    /// <summary>
    /// Read the data property from the JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> containing job
    /// data.</param>
    /// <returns>The data of the job or null.</returns>
    public static string? ReadData(ref Utf8JsonReader reader)
    {
        return reader.GetString();
    }

    /// <summary>
    /// Read the jid property from the JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> containing job
    /// data.</param>
    /// <returns>The JID of the job or null.</returns>
    public static string? ReadJid(ref Utf8JsonReader reader)
    {
        return reader.GetString();
    }

    /// <summary>
    /// Read the priority property from the JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> containing job
    /// data.</param>
    /// <returns>The priority of the job.</returns>
    public static int ReadPriority(ref Utf8JsonReader reader)
    {
        return reader.GetInt32();
    }

    /// <summary>
    /// Read the queue property from the JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> containing job
    /// data.</param>
    /// <returns>The queue name of the job or null.</returns>
    public static string? ReadQueueName(ref Utf8JsonReader reader)
    {
        if (reader.GetString() is not string queue || string.IsNullOrWhiteSpace(queue))
        {
            return null;
        }

        return queue;
    }

    /// <summary>
    /// Read the retries property from the JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> containing job
    /// data.</param>
    /// <returns>The number of retries for the job.</returns>
    public static int ReadRetries(ref Utf8JsonReader reader)
    {
        return reader.GetInt32();
    }

    /// <summary>
    /// Read the state property from the JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> containing job
    /// data.</param>
    /// <returns>The state of the job or null.</returns>
    public static string? ReadState(ref Utf8JsonReader reader)
    {
        return reader.GetString();
    }

    /// <summary>
    /// Read the tags property from the JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> containing job
    /// data.</param>
    /// <returns>The tags of the job or null.</returns>
    public static string[]? ReadTags(ref Utf8JsonReader reader)
    {
        var wasDegenerateObject = JsonConverterHelper.TryConsumeDegenerateObject(
            "tags",
            "array",
            ref reader);
        return wasDegenerateObject
            ? []
            : JsonSerializer.Deserialize<string[]>(ref reader);
    }

    /// <summary>
    /// Read the throttles property from the JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> containing job
    /// data.</param>
    /// <returns>The throttles of the job or null.</returns>
    public static string[]? ReadThrottles(ref Utf8JsonReader reader)
    {
        var wasDegenerateObject = JsonConverterHelper.TryConsumeDegenerateObject(
            "throttles",
            "array",
            ref reader);
        return wasDegenerateObject
            ? []
            : JsonSerializer.Deserialize<string[]>(ref reader);
    }

    /// <summary>
    /// Read the tracked property from the JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> containing job
    /// data.</param>
    /// <returns>The tracked value of the job.</returns>
    public static bool ReadTracked(ref Utf8JsonReader reader)
    {
        return reader.GetBoolean();
    }

    /// <summary>
    /// Write the data property to the JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write job data
    /// to.</param>
    /// <param name="data">The job data.</param>
    public static void WriteData(Utf8JsonWriter writer, string data)
    {
        writer.WriteString("data", data);
    }

    /// <summary>
    /// Write the jid property to the JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write job data
    /// to.</param>
    /// <param name="jid">The job ID.</param>
    public static void WriteJid(Utf8JsonWriter writer, string jid)
    {
        writer.WriteString("jid", jid);
    }

    /// <summary>
    /// Write the klass property to the JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write job data
    /// to.</param>
    /// <param name="className">The job class name.</param>
    public static void WriteClassName(Utf8JsonWriter writer, string className)
    {
        writer.WriteString("klass", className);
    }

    /// <summary>
    /// Write the priority property to the JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write job data
    /// to.</param>
    /// <param name="priority">The job priority.</param>
    public static void WritePriority(Utf8JsonWriter writer, int priority)
    {
        writer.WriteNumber("priority", priority);
    }

    /// <summary>
    /// Write the queue property to the JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write job data
    /// to.</param>
    /// <param name="queueName">The job queue name.</param>
    public static void WriteQueueName(Utf8JsonWriter writer, string? queueName)
    {
        writer.WriteString("queue", queueName);
    }

    /// <summary>
    /// Write the retries property to the JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write job data
    /// to.</param>
    /// <param name="retries">The job retries count.</param>
    public static void WriteRetries(Utf8JsonWriter writer, int retries)
    {
        writer.WriteNumber("retries", retries);
    }

    /// <summary>
    /// Write the state property to the JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write job data
    /// to.</param>
    /// <param name="state">The job state.</param>
    public static void WriteState(Utf8JsonWriter writer, string state)
    {
        writer.WriteString("state", state);
    }

    /// <summary>
    /// Write the tags property to the JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write job data
    /// to.</param>
    /// <param name="tags">The job tags value.</param>
    public static void WriteTags(Utf8JsonWriter writer, string[] tags)
    {
        writer.WritePropertyName("tags");
        JsonSerializer.Serialize(writer, tags);
    }

    /// <summary>
    /// Write the throttles property to the JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write job data
    /// to.</param>
    /// <param name="throttles">The job throttles value.</param>
    public static void WriteThrottles(Utf8JsonWriter writer, string[] throttles)
    {
        writer.WritePropertyName("throttles");
        JsonSerializer.Serialize(writer, throttles);
    }

    /// <summary>
    /// Write the tracked property to the JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write job data
    /// to.</param>
    /// <param name="tracked">The job tracked value.</param>
    public static void WriteTracked(Utf8JsonWriter writer, bool tracked)
    {
        writer.WriteBoolean("tracked", tracked);
    }
}
