using Reqless.Client.Models;
using Reqless.Tests.TestHelpers;

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
            "when"
        );
    }

    /// <summary>
    /// The constructor should throw an ArgumentNullException if queueName is null.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentNullExceptionThrownIfQueueNameIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new PutEvent(1, null!)
        );
        Assert.Equal("queueName", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'queueName')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an ArgumentException if queueName is empty or
    /// composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentExceptionThrownIfQueueNameIsEmptyOrWhitespace()
    {
        foreach (var invalidQueueName in new string[] { "", " ", "\t" })
        {
            var exception = Assert.Throws<ArgumentException>(
                () => new PutEvent(1, invalidQueueName)
            );
            Assert.Equal("queueName", exception.ParamName);
            Assert.Equal(
                $"The value cannot be an empty string or composed entirely of whitespace. (Parameter 'queueName')",
                exception.Message
            );
        }
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