using System.Text.Json.Serialization;

namespace Reqless.Client.Models;

/// <summary>
/// Data object representing the event that occurs when a job is failed.
/// </summary>
public class FailedEvent : JobEvent
{
    /// <summary>
    /// The kind of failure that occurred.
    /// </summary>
    [JsonPropertyName("group")]
    public string Group { get; }

    /// <summary>
    /// The name of the worker that was processing the job when it failed.
    /// </summary>
    [JsonPropertyName("worker")]
    public string WorkerName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FailedEvent"/> class.
    /// </summary>
    /// <param name="when">The time at which the failure occurred.</param>
    /// <param name="group">The kind of failure that occurred.</param>
    /// <param name="workerName">The name of the worker that was processing the
    /// job when it failed.</param>j
    /// <exception cref="ArgumentException">Thrown when <paramref name="when"/>
    /// is less than 0.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="group"/>
    /// or <paramref name="workerName"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="group"/>
    /// or <paramref name="workerName"/> is empty or whitespace.</exception>
    public FailedEvent(
        long when,
        string group,
        string workerName
    ) : base("failed", when)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(group, nameof(group));
        ArgumentException.ThrowIfNullOrWhiteSpace(workerName, nameof(workerName));

        Group = group;
        WorkerName = workerName;
    }
}