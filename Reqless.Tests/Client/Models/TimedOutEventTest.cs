using Reqless.Client.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.Models;

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
    public void Constructor_When_ThrowsWhenNegative()
    {
        Scenario.ThrowsWhenArgumentIsNegative(
            (long invalidWhen) => new TimedOutEvent(invalidWhen),
            "when"
        );
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