using Reqless.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Models;

/// <summary>
/// Tests for the <see cref="WorkerCounts"/> class.
/// </summary>
public class WorkerCountsTest
{
    /// <summary>
    /// The constructor should throw if the given worker name is null, empty, or
    /// only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsIfNameIsNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidName) => new WorkerCounts
            {
                Jobs = 0,
                Stalled = 0,
                WorkerName = invalidName!,
            },
            "WorkerName"
        );
    }

    /// <summary>
    /// The constructor should initialize the name property.
    /// </summary>
    [Fact]
    public void Constructor_InitializesWorkerName()
    {
        var workerName = "test-worker";
        var workerCounts = new WorkerCounts
        {
            Jobs = 0,
            Stalled = 0,
            WorkerName = workerName,
        };
        Assert.Equal(workerName, workerCounts.WorkerName);
    }
}