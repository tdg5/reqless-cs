using System.Text.Json.Serialization;

namespace Reqless.Models.JobEvents;

/// <summary>
/// Data object representing the event that occurs when a job has exhausted all
/// of its retries.
/// </summary>
public class FailedRetriesEvent : JobEvent
{
    /// <summary>
    /// The kind of failure that occurred.
    /// </summary>
    [JsonPropertyName("group")]
    public string Group { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FailedRetriesEvent"/> class.
    /// </summary>
    /// <param name="when">The time at which the failure occurred.</param>
    /// <param name="group">The kind of failure that occurred.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="when"/>
    /// is less than 0.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="group"/>
    /// is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="group"/>
    /// is empty or whitespace.</exception>
    public FailedRetriesEvent(
        long when,
        string group
    ) : base("failed-retries", when)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(group, nameof(group));

        Group = group;
    }
}