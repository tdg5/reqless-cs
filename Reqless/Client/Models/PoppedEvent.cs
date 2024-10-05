using System.Text.Json.Serialization;

namespace Reqless.Client.Models;

/// <summary>
/// Data object representing the event that occurs when a job is popped from a
/// queue.
/// </summary>
public class PoppedEvent : JobEvent
{
    /// <summary>
    /// The name of the worker that popped the job.
    /// </summary>
    [JsonPropertyName("worker")]
    public string WorkerName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PoppedEvent"/> class.
    /// </summary>
    /// <param name="when">The time at which the job was popped.</param>
    /// <param name="workerName">The name of the worker that popped the job.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="when"/>
    /// is less than 0.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="workerName"/>
    /// is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="workerName"/>
    /// is empty or whitespace.</exception>
    public PoppedEvent(
        long when,
        string workerName
    ) : base("popped", when)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workerName, nameof(workerName));

        WorkerName = workerName;
    }
}
