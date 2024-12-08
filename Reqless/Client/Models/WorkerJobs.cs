using Reqless.Client.Serialization;
using Reqless.Common.Validation;
using System.Text.Json.Serialization;

namespace Reqless.Client.Models;

/// <summary>
/// Represents the IDs of jobs belonging to a worker that have or have not
/// expired.
/// </summary>
[JsonConverter(typeof(WorkerJobsJsonConverter))]
public class WorkerJobs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkerJobs"/> class.
    /// </summary>
    /// <param name="jobs">The IDs of the unexpired jobs the worker is responsible for.</param>
    /// <param name="stalled">The IDs of the expired jobs the worker is responsible for.</param>
    public WorkerJobs(string[] jobs, string[] stalled)
    {
        ArgumentNullException.ThrowIfNull(jobs, nameof(jobs));
        ArgumentValidation.ThrowIfAnyNullOrWhitespace(jobs, nameof(jobs));
        ArgumentNullException.ThrowIfNull(stalled, nameof(stalled));
        ArgumentValidation.ThrowIfAnyNullOrWhitespace(stalled, nameof(stalled));
        Jobs = jobs;
        Stalled = stalled;
    }

    /// <summary>
    /// Gets the IDs of the unexpired jobs the worker is responsible for.
    /// </summary>
    [JsonPropertyName("jobs")]
    public string[] Jobs { get; init; }

    /// <summary>
    /// Gets the IDs of the expired jobs the worker is responsible for.
    /// </summary>
    [JsonPropertyName("stalled")]
    public string[] Stalled { get; init; }
}
