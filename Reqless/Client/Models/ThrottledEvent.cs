using System.Text.Json.Serialization;

namespace Reqless.Client.Models;

/// <summary>
/// Data object representing the event that occurs when a job is throttled due
/// to concurrency restrictions.
/// </summary>
public class ThrottledEvent : JobEvent
{
    /// <summary>
    /// The name of the queue that the job was throttled on.
    /// </summary>
    [JsonPropertyName("queue")]
    public string QueueName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrottledEvent"/> class.
    /// </summary>
    /// <param name="when">The time at which the job was throttled.</param>
    /// <param name="queueName">The name of the queue that the job was throttled on.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="when"/>
    /// is less than 0.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="queueName"/>
    /// is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="queueName"/>
    /// is empty or whitespace.</exception>
    public ThrottledEvent(
        long when,
        string queueName
    ) : base("throttled", when)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName, nameof(queueName));

        QueueName = queueName;
    }
}