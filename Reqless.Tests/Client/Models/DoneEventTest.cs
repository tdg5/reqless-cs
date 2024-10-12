using Reqless.Client.Models;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.Models;

/// <summary>
/// Tests for the <see cref="DoneEvent"/> class.
/// </summary>
public class DoneEventTest
{
    /// <summary>
    /// The constructor should throw an ArgumentException if the when parameter
    /// is negative.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentOutOfRangeExceptionThrownIfWhenIsNegative()
    {
        Scenario.ThrowsWhenArgumentIsNegative(
            (long invalidWhen) => new DoneEvent(invalidWhen),
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
        var subject = new DoneEvent(when);
        Assert.Equal("done", subject.What);
        Assert.Equal(when, subject.When);
    }
}
