using Reqless.Client.Models;
using Reqless.Tests.Common.Client.Models;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.Models;

/// <summary>
/// A test class for the <see cref="TrackedJobsResult"/> class.
/// </summary>
public class TrackedJobsResultTest
{
    /// <summary>
    /// <see cref="TrackedJobsResult(Job[], string[])"/> should throw when jobs
    /// is null.
    /// </summary>
    [Fact]
    public void Constructor_Jobs_ThrowsWhenNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new TrackedJobsResult(expiredJids: [], jobs: null!),
            "jobs"
        );
    }

    /// <summary>
    /// <see cref="TrackedJobsResult(Job[], string[])"/> should throw when jobs
    /// includes a null value.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenJobsIncludesNull()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            new TrackedJobsResult(expiredJids: [], jobs: [null!]);
        });
        Assert.Equal("Value cannot include null. (Parameter 'jobs')", exception.Message);
    }

    /// <summary>
    /// <see cref="TrackedJobsResult(Job[], string[])"/> should throw when
    /// expiredJids is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenExpiredJidsIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new TrackedJobsResult(expiredJids: null!, jobs: []),
            "expiredJids"
        );
    }

    /// <summary>
    /// <see cref="TrackedJobsResult(Job[], string[])"/> should throw when
    /// expiredJids includes values that are null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenExpiredJidsIncludesNullEmptyOrOnlyWhitespaceValue()
    {
        Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespace(
            (invalidJid) => new TrackedJobsResult(
                expiredJids: [invalidJid!],
                jobs: []
            ),
            "expiredJids"
        );
    }

    /// <summary>
    /// <see cref="TrackedJobsResult(Job[], string[])"/> should assign
    /// properties appropriately.
    /// </summary>
    [Fact]
    public void Constructor_AssignsProperties()
    {
        var jobs = new Job[] { JobFactory.NewJob() };
        var expiredJids = new string[] { "expiredJid" };
        var result = new TrackedJobsResult(jobs, expiredJids);
        Assert.Equal(jobs, result.Jobs);
        Assert.Equal(expiredJids, result.ExpiredJids);
    }
}
