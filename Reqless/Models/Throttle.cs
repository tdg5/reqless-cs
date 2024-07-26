using System.Text.Json.Serialization;

namespace Reqless.Models;

/// <summary>
/// Represents a throttle for limiting job concurrency.
/// </summary>
public class Throttle
{
    /// <summary>
    /// The unique identifier of the throttle.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The maximum number of jobs that can hold the throttle at once.
    /// </summary>
    [JsonPropertyName("maximum")]
    public required int Maximum { get; init; }

    /// <summary>
    /// Create a new <see cref="Throttle"/> instance.
    /// </summary>
    public Throttle()
    {
    }
}