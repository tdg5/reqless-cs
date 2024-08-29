using System.Text.Json.Serialization;
using Reqless.Common.Validation;

namespace Reqless.Client.Models;

/// <summary>
/// Represents a pattern of queue identifiers that are relatively prioritized.
/// When fairly is true, queues matching the pattern are all considered to be of
/// equivalent priority. When fairly is false, the queues are prioritized with
/// the left most queue being of highest releative priority and the right most
/// queue being of lowest relative priority.
/// </summary>
public class QueuePriorityPattern
{
    /// <summary>
    /// A list of queue identifiers that are used to match queues that should be
    /// included in this prioritization grouping.
    /// </summary>
    [JsonPropertyName("pattern")]
    public List<string> Pattern { get; }

    /// <summary>
    /// Whether the queues matching the pattern are considered to be of equivalent
    /// priority. When true, matching queues should be serviced in a round-robin
    /// fashion. When false, matching queues are prioritized from highest to
    /// lowest from left to right, respectively.
    /// </summary>
    [JsonPropertyName("fairly")]
    public bool Fairly { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueuePriorityPattern"/> class.
    /// </summary>
    public QueuePriorityPattern(
        List<string> pattern,
        bool fairly = false
    )
    {
        ArgumentValidation.ThrowIfAnyNullOrWhitespace(pattern, nameof(pattern));
        Pattern = pattern;
        Fairly = fairly;
    }
}