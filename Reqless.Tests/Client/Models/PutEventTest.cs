using Reqless.Client.Models;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.Models;

/// <summary>
/// Tests for the <see cref="PutEvent"/> class.
/// </summary>
public class PutEventTest
{
    /// <summary>
    /// The constructor should throw an ArgumentOutOfRangeException if when is
    /// negative.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentOutOfRangeExceptionThrownIfWhenIsNegative()
    {
        Scenario.ThrowsWhenArgumentIsNegative(
            (long invalidWhen) => new PutEvent(invalidWhen, "QueueName"),
            "when");
    }

    /// <summary>
    /// The constructor should throw an ArgumentNullException if queueName is null.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentNullExceptionThrownIfQueueNameIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new PutEvent(1, null!), "queueName");
    }

    /// <summary>
    /// The constructor should throw an ArgumentException if queueName is empty or
    /// composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentExceptionThrownIfQueueNameIsEmptyOrWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidQueueName) => new PutEvent(1, invalidQueueName!),
            "queueName");
    }

    /// <summary>
    /// The constructor should set the What, When, and QueueName properties
    /// appropriately.
    /// </summary>
    [Fact]
    public void Constructor_SetsWhenAndWhatAndQueueNameAppropriately()
    {
        var when = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var queueName = "QueueName";
        var subject = new PutEvent(when, queueName);
        Assert.Equal("put", subject.What);
        Assert.Equal(when, subject.When);
        Assert.Equal(queueName, subject.QueueName);
    }
}
