using Reqless.Client.Serialization;
using Reqless.Common.Validation;
using System.Text.Json.Serialization;

namespace Reqless.Client.Models;

/// <summary>
/// Data class representing the result of a request for all tracked jobs.
/// </summary>
[JsonConverter(typeof(TrackedJobsResultJsonConverter))]
public class TrackedJobsResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TrackedJobsResult"/> class.
    /// </summary>
    /// <param name="jobs">The jobs that are currently being tracked.</param>
    /// <param name="expiredJids">The JIDs of tracked jobs that no additional
    /// information is available for.</param>
    public TrackedJobsResult(Job[] jobs, string[] expiredJids)
    {
        ArgumentValidation.ThrowIfAnyNull(jobs, nameof(jobs));
        ArgumentValidation.ThrowIfAnyNullOrWhitespace(expiredJids, nameof(expiredJids));

        ExpiredJids = expiredJids;
        Jobs = jobs;
    }

    /// <summary>
    /// Gets the JIDs of tracked jobs that no additional information is available
    /// for.
    /// </summary>
    public string[] ExpiredJids { get; }

    /// <summary>
    /// Gets the jobs that are currently being tracked.
    /// </summary>
    public Job[] Jobs { get; }
}
