using Reqless.Models.JobEvents;

namespace Reqless.Tests.Models.JobEvents;

/// <summary>
/// Tests for the <see cref="FailedEvent"/> class.
/// </summary>
public class FailedEventTest
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
            () => new FailedEvent(invalidWhen, "Group", "WorkerName")
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
    /// The constructor should throw an ArgumentNullException if the group parameter
    /// is null.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentNullExceptionThrownIfGroupIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new FailedEvent(1, null!, "WorkerName")
        );
        Assert.Equal("group", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'group')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an ArgumentException if the group parameter
    /// is empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentExceptionThrownIfGroupIsEmptyOrWhitespace()
    {
        foreach (var invalidGroup in new string[] { "", " ", "\t" })
        {
            var exception = Assert.Throws<ArgumentException>(
                () => new FailedEvent(1, invalidGroup, "WorkerName")
            );
            Assert.Equal("group", exception.ParamName);
            Assert.Equal(
                $"The value cannot be an empty string or composed entirely of whitespace. (Parameter 'group')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an ArgumentNullException if the workerName parameter
    /// is null.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentNullExceptionThrownIfWorkerNameIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new FailedEvent(1, "Group", null!)
        );
        Assert.Equal("workerName", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'workerName')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an ArgumentException if the workerName parameter
    /// is empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentExceptionThrownIfWorkerNameIsEmptyOrWhitespace()
    {
        foreach (var invalidWorkerName in new string[] { "", " ", "\t" })
        {
            var exception = Assert.Throws<ArgumentException>(
                () => new FailedEvent(1, "Group", invalidWorkerName)
            );
            Assert.Equal("workerName", exception.ParamName);
            Assert.Equal(
                $"The value cannot be an empty string or composed entirely of whitespace. (Parameter 'workerName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should set the When, What, Group, and WorkerName
    /// properties appropriately.
    /// </summary>
    [Fact]
    public void Constructor_SetsWhenAndWhatAndGroupAndWorkerNameAppropriately()
    {
        var when = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var group = "Group";
        var workerName = "WorkerName";
        var subject = new FailedEvent(when, group, workerName);
        Assert.Equal("failed", subject.What);
        Assert.Equal(when, subject.When);
        Assert.Equal(group, subject.Group);
        Assert.Equal(workerName, subject.WorkerName);
    }
}