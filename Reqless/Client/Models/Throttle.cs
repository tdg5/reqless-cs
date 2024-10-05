using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Reqless.Client.Models;

/// <summary>
/// Represents a throttle for limiting job concurrency.
/// </summary>
public class Throttle
{
    private string _Id;

    /// <summary>
    /// The unique identifier of the throttle.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id
    {
        get => _Id;

        [MemberNotNull(nameof(_Id))]
        init
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Id));
            _Id = value;
        }
    }

    /// <summary>
    /// The maximum number of jobs that can hold the throttle at once.
    /// </summary>
    [JsonPropertyName("maximum")]
    public required int Maximum { get; init; }

    /// <summary>
    /// The TTL of the throttle. Values less than 0 indicate the throttle has no
    /// associated expiration.
    /// </summary>
    [JsonPropertyName("ttl")]
    public required int Ttl { get; init; }
}
