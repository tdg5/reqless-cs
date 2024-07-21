using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Reqless.Models;

/// <summary>
/// Represents the counts of the number of jobs in various states of a queue.
/// </summary>
public class QueueCounts
{
    private string _QueueName;

    /// <summary>
    /// The number of jobs that are currently waiting on dependencies before
    /// they can be processed.
    /// </summary>
    [JsonPropertyName("depends")]
    public required int Depends { get; init; }

    /// <summary>
    /// A flag indicating whether the queue is paused.
    /// </summary>
    [JsonPropertyName("paused")]
    public required bool Paused { get; init; }

    /// <summary>
    /// The name of the queue.
    /// </summary>
    [JsonPropertyName("name")]
    public required string QueueName
    {
        get => _QueueName;

        [MemberNotNull(nameof(_QueueName))]
        init
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(QueueName));
            _QueueName = value;
        }
    }

    /// <summary>
    /// The number of recurring jobs currently registered with the queue.
    /// </summary>
    [JsonPropertyName("recurring")]
    public required int Recurring { get; init; }

    /// <summary>
    /// The number of jobs that are currently running.
    /// </summary>
    [JsonPropertyName("running")]
    public required int Running { get; init; }

    /// <summary>
    /// The number of jobs that are currently scheduled to run.
    /// </summary>
    [JsonPropertyName("scheduled")]
    public required int Scheduled { get; init; }

    /// <summary>
    /// The number of jobs that recently timed out and are pending to be
    /// retried.
    /// </summary>
    [JsonPropertyName("stalled")]
    public required int Stalled { get; init; }

    /// <summary>
    /// The number of jobs that blocked from running due to a lack of available
    /// throttles.
    /// </summary>
    [JsonPropertyName("throttled")]
    public required int Throttled { get; init; }

    /// <summary>
    /// The number of jobs that are currently able and waiting to be processed.
    /// </summary>
    [JsonPropertyName("waiting")]
    public required int Waiting { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueCounts"/> class.
    /// </summary>
    public QueueCounts()
    {
    }
}