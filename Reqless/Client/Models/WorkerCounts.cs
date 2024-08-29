using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Reqless.Client.Models;

/// <summary>
/// Represents the counts of the number of jobs belonging to a worker that have
/// or have not expired.
/// </summary>
public class WorkerCounts
{
    private string _WorkerName;

    /// <summary>
    /// The number of jobs that belong to the worker that have not yet expired.
    /// </summary>
    [JsonPropertyName("jobs")]
    public required int Jobs { get; init; }

    /// <summary>
    /// The number of jobs that belong to the worker and have expired.
    /// </summary>
    [JsonPropertyName("stalled")]
    public required int Stalled { get; init; }

    /// <summary>
    /// The name of the worker.
    /// </summary>
    [JsonPropertyName("name")]
    public required string WorkerName
    {
        get => _WorkerName;

        [MemberNotNull(nameof(_WorkerName))]
        init
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(WorkerName));
            _WorkerName = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkerCounts"/> class.
    /// </summary>
    public WorkerCounts()
    {
    }
}