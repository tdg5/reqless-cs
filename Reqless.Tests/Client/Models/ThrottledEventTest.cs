using Reqless.Client.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.Models;

/// <summary>
/// Tests for the <see cref="ThrottledEvent"/> class.
/// </summary>
public class ThrottledEventTest
{
    /// <summary>
    /// The constructor should throw an ArgumentOutOfRangeException if the when
    /// parameter is negative.
    /// </summary>
    [Fact]
    public void Constructor_When_ThrowsIfNegative()
    {
        Scenario.ThrowsWhenArgumentIsNegative(
            (long invalidWhen) => new ThrottledEvent(invalidWhen, "QueueName"),
            "when"
        );
    }

    /// <summary>
    /// The constructor should throw an ArgumentNullException if queueName is
    /// null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_QueueName_ThrowsIfNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidQueueName) => new ThrottledEvent(1, invalidQueueName!),
            "queueName"
        );
    }

    /// <summary>
    /// The constructor should set the When and What and QueueName properties
    /// appropriately.
    /// </summary>
    [Fact]
    public void Constructor_SetsWhenAndWhatAndQueueNameAppropriately()
    {
        var when = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var queueName = "QueueName";
        var subject = new ThrottledEvent(when, queueName);
        Assert.Equal("throttled", subject.What);
        Assert.Equal(when, subject.When);
        Assert.Equal(queueName, subject.QueueName);
    }
}