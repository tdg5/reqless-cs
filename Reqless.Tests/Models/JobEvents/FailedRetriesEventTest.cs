using Reqless.Models.JobEvents;

namespace Reqless.Tests.Models.JobEvents;

/// <summary>
/// Tests for the <see cref="FailedRetriesEvent"/> class.
/// </summary>
public class FailedRetriesEventTest
{
    /// <summary>
    /// The constuctor should throw an ArgumentOutOfRangeException if the when
    /// parameter is negative.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentOutOfRangeExceptionThrownIfWhenIsNegative()
    {
        var invalidWhen = -1;
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new FailedRetriesEvent(invalidWhen, "Group")
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
            () => new FailedRetriesEvent(1, null!)
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
                () => new FailedRetriesEvent(1, invalidGroup)
            );
            Assert.Equal("group", exception.ParamName);
            Assert.Equal(
                $"The value cannot be an empty string or composed entirely of whitespace. (Parameter 'group')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should set the What, When, and Group properties
    /// appropriately.
    /// </summary>
    [Fact]
    public void Constructor_SetsWhenAndWhatAndGroupAppropriately()
    {
        var when = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var group = "Group";
        var subject = new FailedRetriesEvent(when, group);
        Assert.Equal("failed-retries", subject.What);
        Assert.Equal(when, subject.When);
        Assert.Equal(group, subject.Group);
    }
}