using Reqless.Client.Serialization;
using Reqless.Common.Validation;
using System.Text.Json.Serialization;

namespace Reqless.Client.Models;

/// <summary>
/// Data object representing an event that a job encounters in its lifetime.
/// </summary>
[JsonConverter(typeof(JobEventJsonConverter))]
public class JobEvent
{
    /// <summary>
    /// The type of event that occurred.
    /// </summary>
    [JsonPropertyName("what")]
    public string What { get; }

    /// <summary>
    /// The time at which the event occurred.
    /// </summary>
    [JsonPropertyName("when")]
    public long When { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JobEvent"/> class.
    /// </summary>
    /// <param name="what">The type of event that occurred.</param>
    /// <param name="when">The time at which the event occurred.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="when"/>
    /// is less than 0.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="what"/>
    /// is null.</exception>
    protected JobEvent(string what, long when)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(what, nameof(what));
        ArgumentValidation.ThrowIfNegative(when, nameof(when));

        What = what;
        When = when;
    }
}