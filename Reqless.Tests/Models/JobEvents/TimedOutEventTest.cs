using Reqless.Models.JobEvents;

namespace Reqless.Tests.Models.JobEvents;

/// <summary>
/// Tests for the <see cref="TimedOutEvent"/> class.
/// </summary>
public class TimedOutEventTest
{
    /// <summary>
    /// The constructor should throw an ArgumentOutOfRangeException if the when
    /// parameter is negative.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentOutOfRangeExceptionThrownIfWhenIsNegative()
    {
        var invalidWhen = -1;
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new TimedOutEvent(invalidWhen)
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
    /// The constructor should set the What and When properties appropriately.
    /// </summary>
    [Fact]
    public void Constructor_SetsWhenAndWhatAppropriately()
    {
        var when = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var subject = new TimedOutEvent(when);
        Assert.Equal("timed-out", subject.What);
        Assert.Equal(when, subject.When);
    }
}