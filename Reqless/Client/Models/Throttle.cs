using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Reqless.Client.Models;

/// <summary>
/// Represents a throttle for limiting job concurrency.
/// </summary>
public class Throttle
{
    private string _id;

    /// <summary>
    /// Gets the unique identifier of the throttle.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id
    {
        get => _id;

        [MemberNotNull(nameof(_id))]
        init
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Id));
            _id = value;
        }
    }

    /// <summary>
    /// Gets the maximum number of jobs that can hold the throttle at once.
    /// </summary>
    [JsonPropertyName("maximum")]
    public required int Maximum { get; init; }

    /// <summary>
    /// Gets the TTL of the throttle. Values less than 0 indicate the throttle has no
    /// associated expiration.
    /// </summary>
    [JsonPropertyName("ttl")]
    public required int Ttl { get; init; }
}
