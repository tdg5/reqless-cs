using Reqless.Common.Validation;

using System.Text.Json.Serialization;

namespace Reqless.Client.Models;

/// <summary>
/// Data class representing a job failure, as defined by reqless-core.
/// </summary>
public class JobFailure
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JobFailure"/> class.
    /// </summary>
    /// <param name="group">The kind of failure that occurred.</param>
    /// <param name="message">The message associated with the failure.</param>
    /// <param name="when">The time at which the failure occurred.</param>
    /// <param name="workerName">The name of the worker that encountered the failure.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref
    /// name="group"/>, <paramref name="message"/>, or <paramref
    /// name="workerName"/> is empty, or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref
    /// name="when"/> is less than or equal to 0.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="group"/>,
    /// <paramref name="message"/>, or <paramref name="workerName"/> is null.</exception>
    public JobFailure(string group, string message, long when, string workerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(group, nameof(group));
        ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));
        ArgumentException.ThrowIfNullOrWhiteSpace(workerName, nameof(workerName));
        ArgumentValidation.ThrowIfNotPositive(when, nameof(when));

        Group = group;
        Message = message;
        WorkerName = workerName;
        When = when;
    }

    /// <summary>
    /// Gets the kind of failure that occurred.
    /// </summary>
    [JsonPropertyName("group")]
    public string Group { get; }

    /// <summary>
    /// Gets the message associated with the failure.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; }

    /// <summary>
    /// Gets the time at which the failure occurred.
    /// </summary>
    [JsonPropertyName("when")]
    public long When { get; }

    /// <summary>
    /// Gets the name of the worker that encountered the failure.
    /// </summary>
    [JsonPropertyName("worker")]
    public string WorkerName { get; }
}
