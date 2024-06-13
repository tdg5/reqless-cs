using System.Text.Json.Serialization;

namespace Reqless.Models.JobEvents;

/// <summary>
/// Data object representing the event that occurs when a job is put onto a
/// queue.
/// </summary>
public class PutEvent : JobEvent
{
    /// <summary>
    /// The name of the queue onto which the job was put.
    /// </summary>
    [JsonPropertyName("queue")]
    public string QueueName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PutEvent"/> class.
    /// </summary>
    /// <param name="when">The time at which the job was put onto the queue.</param>
    /// <param name="queueName">The name of the queue onto which the job was put.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="when"/>
    /// is less than 0.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="queueName"/>
    /// is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="queueName"/>
    /// is empty or whitespace.</exception>
    public PutEvent(
        long when,
        string queueName
    ) : base("put", when)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName, nameof(queueName));

        QueueName = queueName;
    }
}