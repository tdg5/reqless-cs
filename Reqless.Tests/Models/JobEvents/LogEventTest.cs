using System.Text.Json;
using Reqless.Models.JobEvents;

namespace Reqless.Tests.Models.JobEvents;

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
    public void Constructor_ArgumentOutOfRangeExceptionThrownIfWhenIsNegative()
    {
        var invalidWhen = -1;
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new LogEvent("what", invalidWhen)
        );
        Assert.Equal("when", exception.ParamName);
        // Use a similar exception to compose the message to avoid
        // inconsistencies with line endings across platforms.
        var similarException = new ArgumentOutOfRangeException(
            "when",
            invalidWhen,
            "when must be greater than or equal to 0"
        );
        Assert.Equal(similarException.Message, exception.Message);
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