using System.Text.Json;
using Reqless.Models;

namespace Reqless.Tests.Models;

/// <summary>
/// Unit tests for <see cref="Throttle"/>.
/// </summary>
public class ThrottleTest
{
    /// <summary>
    /// <see cref="Throttle"/> should be deserializable from JSON like that
    /// returned by Reqless server.
    /// </summary>
    [Fact]
    public void DeserializingFromJsonReturnsExpectedInstance()
    {
        var id = "throttle-id";
        var maximum = 42;
        var throttleJson = $$"""{"id": "{{id}}", "maximum": {{maximum}}}""";
        var throttle = JsonSerializer.Deserialize<Throttle>(throttleJson);
        Assert.NotNull(throttle);
        Assert.Equal(id, throttle.Id);
        Assert.Equal(maximum, throttle.Maximum);
    }

    /// <summary>
    /// <see cref="Throttle"/> should be serializable to JSON like that
    /// returned by Reqless server.
    /// </summary>
    [Fact]
    public void SerializingToJsonReturnsJsonThatCanBeRoundTripped()
    {
        var expectedThrottle = new Throttle
        {
            Id = "throttle-id",
            Maximum = 42,
        };
        var throttleJson = JsonSerializer.Serialize(expectedThrottle);
        var throttle = (
            JsonSerializer.Deserialize<Throttle>(throttleJson)
        );
        Assert.Equivalent(expectedThrottle, throttle);
    }
}