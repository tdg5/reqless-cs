using System.Text.Json.Serialization;
using Reqless.Serialization;

namespace Reqless.Models;

/// <summary>
/// Data class representing the result of a request for all tracked jobs.
/// </summary>
[JsonConverter(typeof(TrackedJobsResultJsonConverter))]
public class TrackedJobsResult
{
    /// <summary>
    /// The jobs that are currently being tracked.
    /// </summary>
    public Job[] Jobs { get; }

    /// <summary>
    /// The JIDs of tracked jobs that no additional information is available
    /// for.
    /// </summary>
    public string[] ExpiredJids { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TrackedJobsResult"/> class.
    /// </summary>
    /// <param name="jobs">The jobs that are currently being tracked.</param>
    /// <param name="expiredJids">The JIDs of tracked jobs that no additional
    /// information is available for.</param>
    public TrackedJobsResult(Job[] jobs, string[] expiredJids)
    {
        ValidationHelper.ThrowIfAnyNull(jobs, nameof(jobs));
        ValidationHelper.ThrowIfAnyNullOrWhitespace(expiredJids, nameof(expiredJids));

        Jobs = jobs;
        ExpiredJids = expiredJids;
    }
}