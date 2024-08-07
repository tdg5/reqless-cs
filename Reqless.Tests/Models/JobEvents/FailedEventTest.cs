using Reqless.Models.JobEvents;
using Reqless.Tests.TestHelpers;

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
        Scenario.ThrowsWhenParameterIsNegative(
            (long invalidWhen) => new FailedEvent(invalidWhen, "Group", "WorkerName"),
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
        Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespace(
            (invalidGroup) => new FailedEvent(1, invalidGroup!, "WorkerName"),
            "group"
        );
    }

    /// <summary>
    /// The constructor should throw an ArgumentNullException if the workerName
    /// parameter is null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ArgumentNullExceptionThrownIfWorkerNameIsNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespace(
            (invalidWorkerName) => new FailedEvent(1, "Group", invalidWorkerName!),
            "workerName"
        );
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