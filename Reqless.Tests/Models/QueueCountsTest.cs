using Reqless.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Models;

/// <summary>
/// Tests for the <see cref="QueueCounts"/> class.
/// </summary>
public class QueueCountsTest
{
    /// <summary>
    /// The constructor should throw if the given queue name is null, empty, or
    /// only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsIfNameIsNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespace(
            (invalidName) => new QueueCounts
            {
                Depends = 0,
                QueueName = invalidName!,
                Paused = false,
                Recurring = 0,
                Running = 0,
                Scheduled = 0,
                Stalled = 0,
                Throttled = 0,
                Waiting = 0
            },
            "QueueName"
        );
    }

    /// <summary>
    /// The constructor should initialize the name property.
    /// </summary>
    [Fact]
    public void Constructor_InitializesQueueName()
    {
        var queueName = "test-queue";
        var queueCounts = new QueueCounts
        {
            Depends = 0,
            QueueName = queueName,
            Paused = false,
            Recurring = 0,
            Running = 0,
            Scheduled = 0,
            Stalled = 0,
            Throttled = 0,
            Waiting = 0
        };
        Assert.Equal(queueName, queueCounts.QueueName);
    }
}