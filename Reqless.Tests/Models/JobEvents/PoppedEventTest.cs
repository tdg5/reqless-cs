using Reqless.Models.JobEvents;

namespace Reqless.Tests.Models.JobEvents;

/// <summary>
/// Tests for the <see cref="PoppedEvent"/> class.
/// </summary>
public class PoppedEventTest
{
    /// <summary>
    /// The constructor should throw an ArgumentOutOfRangeException if when is
    /// negative.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentOutOfRangeExceptionThrownIfWhenIsNegative()
    {
        var invalidWhen = -1;
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new PoppedEvent(invalidWhen, "WorkerName")
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
    /// The constructor should throw an ArgumentNullException if workerName is null.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentNullExceptionThrownIfWorkerNameIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new PoppedEvent(1, null!)
        );
        Assert.Equal("workerName", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'workerName')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an ArgumentException if workerName is empty or
    /// composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentExceptionThrownIfWorkerNameIsEmptyOrWhitespace()
    {
        foreach (var invalidWorkerName in new string[] { "", " ", "\t" })
        {
            var exception = Assert.Throws<ArgumentException>(
                () => new PoppedEvent(1, invalidWorkerName)
            );
            Assert.Equal("workerName", exception.ParamName);
            Assert.Equal(
                $"The value cannot be an empty string or composed entirely of whitespace. (Parameter 'workerName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should set the What, When, and WorkerName properties
    /// appropriately.
    /// </summary>
    [Fact]
    public void Constructor_SetsWhenAndWhatAndWorkerNameAppropriately()
    {
        var when = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var workerName = "WorkerName";
        var subject = new PoppedEvent(when, workerName);
        Assert.Equal("popped", subject.What);
        Assert.Equal(when, subject.When);
        Assert.Equal(workerName, subject.WorkerName);
    }
}