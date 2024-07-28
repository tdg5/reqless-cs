using Reqless.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Reqless.Models.JobEvents;

/// <summary>
/// Data object representing the event that occurs when a job.log is used to add
/// an event to a job's history.
/// </summary>
[JsonConverter(typeof(LogEventJsonConverter))]
public class LogEvent : JobEvent
{
    /// <summary>
    /// Adhoc data associated with the event.
    /// </summary>
    public Dictionary<string, JsonElement> Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEvent"/> class.
    /// </summary>
    /// <param name="what">The name of the event that occurred.</param>
    /// <param name="when">The time at which the event occurred.</param>
    /// <param name="data">The data associated with the event.</param>
    public LogEvent(
       string what,
       long when,
       Dictionary<string, JsonElement>? data = null
   ) : base(what, when)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(what, nameof(what));

        Data = data ?? [];
    }
}