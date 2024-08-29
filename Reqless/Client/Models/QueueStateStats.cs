using System.Text.Json.Serialization;

namespace Reqless.Client.Models;

/// <summary>
/// Statistics related to the duration of time that jobs spent in the given
/// state of a queue.
/// </summary>
public class QueueStateStats
{
    /// <summary>
    /// The number of jobs that exited the queue state and were able to report
    /// metrics on their time spent in that state.
    /// </summary>
    [JsonPropertyName("count")]
    public required int Count { get; init; }

    /// <summary>
    /// A histogram of durations that jobs have spent in the queue state.
    /// </summary>
    /// <remarks>
    /// The histogram's data points are at the second resolution for the first
    /// minute, the minute resolution for the first hour, the 15-minute
    /// resolution for the first day, the hour resolution for the first 3 days,
    /// and then at the day resolution from there on out.
    /// </remarks>
    [JsonPropertyName("histogram")]
    public required int[] Histogram { get; init; }

    /// <summary>
    /// The average time that jobs have spent in the queue state.
    /// </summary>
    [JsonPropertyName("mean")]
    public required int Mean { get; init; }

    /// <summary>
    /// The standard deviation of the time that jobs have spent in the queue state.
    /// </summary>
    [JsonPropertyName("std")]
    public required int StandardDeviation { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueStateStats"/> class.
    /// </summary>
    public QueueStateStats()
    {
    }
}