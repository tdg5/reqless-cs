using Reqless.Models.JobEvents;
using Reqless.Tests.TestHelpers;

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
    public void Constructor_When_ThrowsWhenNegative()
    {
        Scenario.ThrowsWhenParameterIsNegative(
            (long invalidWhen) => new PoppedEvent(invalidWhen, "WorkerName"),
            "when"
        );
    }

    /// <summary>
    /// The constructor should throw an ArgumentNullException if workerName is
    /// null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_WorkerName_ThrowsWhenNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespace(
            (invalidWorkerName) => new PoppedEvent(1, invalidWorkerName!),
            "workerName"
        );
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