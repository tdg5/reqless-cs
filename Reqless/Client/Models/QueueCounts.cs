using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Reqless.Client.Models;

/// <summary>
/// Represents the counts of the number of jobs in various states of a queue.
/// </summary>
public class QueueCounts
{
    private string _queueName;

    /// <summary>
    /// Gets the number of jobs that are currently waiting on dependencies
    /// before they can be processed.
    /// </summary>
    [JsonPropertyName("depends")]
    public required int Depends { get; init; }

    /// <summary>
    /// Gets a value indicating whether the queue is paused.
    /// </summary>
    [JsonPropertyName("paused")]
    public required bool Paused { get; init; }

    /// <summary>
    /// Gets the name of the queue.
    /// </summary>
    [JsonPropertyName("name")]
    public required string QueueName
    {
        get => _queueName;

        [MemberNotNull(nameof(_queueName))]
        init
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(QueueName));
            _queueName = value;
        }
    }

    /// <summary>
    /// Gets the number of recurring jobs currently registered with the queue.
    /// </summary>
    [JsonPropertyName("recurring")]
    public required int Recurring { get; init; }

    /// <summary>
    /// Gets the number of jobs that are currently running.
    /// </summary>
    [JsonPropertyName("running")]
    public required int Running { get; init; }

    /// <summary>
    /// Gets the number of jobs that are currently scheduled to run.
    /// </summary>
    [JsonPropertyName("scheduled")]
    public required int Scheduled { get; init; }

    /// <summary>
    /// Gets the number of jobs that recently timed out and are pending to be
    /// retried.
    /// </summary>
    [JsonPropertyName("stalled")]
    public required int Stalled { get; init; }

    /// <summary>
    /// Gets the number of jobs that blocked from running due to a lack of available
    /// throttles.
    /// </summary>
    [JsonPropertyName("throttled")]
    public required int Throttled { get; init; }

    /// <summary>
    /// Gets the number of jobs that are currently able and waiting to be processed.
    /// </summary>
    [JsonPropertyName("waiting")]
    public required int Waiting { get; init; }
}
