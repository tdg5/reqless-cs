using System.Diagnostics.CodeAnalysis;

namespace Reqless.Models;

/// <summary>
/// Represents the counts of the number of jobs in various states of a queue.
/// </summary>
public class QueueCounts
{
    private string _Name;

    /// <summary>
    /// The number of jobs that are currently waiting on dependencies before
    /// they can be processed.
    /// </summary>
    public required int Depends { get; init; }

    /// <summary>
    /// The name of the queue.
    /// </summary>
    public required string Name
    {
        get => _Name;

        [MemberNotNull(nameof(_Name))]
        init
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Name));
            _Name = value;
        }
    }

    /// <summary>
    /// A flag indicating whether the queue is paused.
    /// </summary>
    public required bool Paused { get; init; }

    /// <summary>
    /// The number of recurring jobs currently registered with the queue.
    /// </summary>
    public required int Recurring { get; init; }

    /// <summary>
    /// The number of jobs that are currently running.
    /// </summary>
    public required int Running { get; init; }

    /// <summary>
    /// The number of jobs that are currently scheduled to run.
    /// </summary>
    public required int Scheduled { get; init; }

    /// <summary>
    /// The number of jobs that recently timed out and are pending to be
    /// retried.
    /// </summary>
    public required int Stalled { get; init; }

    /// <summary>
    /// The number of jobs that blocked from running due to a lack of available
    /// throttles.
    /// </summary>
    public required int Throttled { get; init; }

    /// <summary>
    /// The number of jobs that are currently able and waiting to be processed.
    /// </summary>
    public required int Waiting { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueCounts"/> class.
    /// </summary>
    public QueueCounts()
    {
    }
}