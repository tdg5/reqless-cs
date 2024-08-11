using Reqless.Models.JobEvents;
using Reqless.Tests.TestHelpers;

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
        Scenario.ThrowsWhenArgumentIsNegative(
            (long invalidWhen) => new FailedRetriesEvent(invalidWhen, "Group"),
            "when"
        );
    }

    /// <summary>
    /// The constructor should throw an ArgumentNullException if the group parameter
    /// is null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentNullExceptionThrownIfGroupIsNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidGroup) => new FailedRetriesEvent(1, invalidGroup!),
            "group"
        );
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