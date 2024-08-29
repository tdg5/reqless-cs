using System.Text.Json;
using Reqless.Client.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.Models;

/// <summary>
/// Tests for the <see cref="LogEvent"/> class.
/// </summary>
public class LogEventTest
{
    /// <summary>
    /// The constructor should throw an ArgumentException if the when parameter
    /// is negative.
    /// </summary>
    [Fact]
    public void Constructor_When_ThrowsIfNegative()
    {
        Scenario.ThrowsWhenArgumentIsNegative(
            (long invalidWhen) => new LogEvent("what", invalidWhen),
            "when"
        );
    }

    /// <summary>
    /// The constructor should throw an ArgumentException if the what parameter
    /// is null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_What_ThrowsIfNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidWhat) => new LogEvent(invalidWhat!, 123),
            "what"
        );
    }

    /// <summary>
    /// The constructor should set the What and When properties appropriately
    /// when given no data.
    /// </summary>
    [Fact]
    public void Constructor_SetsWhenAndWhatAppropriatelyWithNoData()
    {
        var what = "what";
        var when = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var subject = new LogEvent(what, when);
        Assert.Equal(what, subject.What);
        Assert.Equal(when, subject.When);
        Assert.Empty(subject.Data);
    }

    /// <summary>
    /// The constructor should set the What and When properties appropriately
    /// when given data.
    /// </summary>
    [Fact]
    public void Constructor_SetsWhenAndWhatAppropriatelyWithData()
    {
        var what = "what";
        var when = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var extra = "\"extra\"";
        var extraJsonElement = JsonDocument.Parse(extra).RootElement;
        var subject = new LogEvent(what, when, new Dictionary<string, JsonElement>
        {
            { "extra", extraJsonElement }
        });
        Assert.Equal(what, subject.What);
        Assert.Equal(when, subject.When);
        Assert.Equal("extra", subject.Data["extra"].GetString());
    }
}