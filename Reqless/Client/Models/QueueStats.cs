using System.Text.Json.Serialization;

namespace Reqless.Client.Models;

/// <summary>
/// Represents statistics on the durations that jobs spent in different states
/// for the respective queue on the respective date.
/// </summary>
public class QueueStats
{
    /// <summary>
    /// The number of jobs that failed and never exited the failed state for the
    /// respective queue and respective date.
    /// </summary>
    /// <remarks>
    /// If a job failed and was requeued, it would not be represented in this count.
    /// </remarks>
    [JsonPropertyName("failed")]
    public required int Failed { get; init; }

    /// <summary>
    /// The total number of jobs that failed for the respective queue and
    /// respective date.
    /// </summary>
    /// <remarks>
    /// If a job failed and was requeued, it would still be represented in this
    /// count.
    /// </remarks>
    [JsonPropertyName("failures")]
    public required int Failures { get; init; }

    /// <summary>
    /// The total number of retries that occurred for the respective queue and
    /// respective date.
    /// </summary>
    [JsonPropertyName("retries")]
    public required int Retries { get; init; }

    /// <summary>
    /// More granular statistics related to the duration of time that jobs spent
    /// in the running state for the respective queue and respective date.
    /// </summary>
    [JsonPropertyName("run")]
    public required QueueStateStats Run { get; set; }

    /// <summary>
    /// More granular statistics related to the duration of time that jobs spent
    /// in the waiting state for the respective queue and respective date.
    /// </summary>
    [JsonPropertyName("wait")]
    public required QueueStateStats Wait { get; set; }
}
