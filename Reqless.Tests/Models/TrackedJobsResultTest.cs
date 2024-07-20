using Reqless.Models;
using Reqless.Tests.TestHelpers;
using Reqless.Tests.TestHelpers.Factories;

namespace Reqless.Tests.Models;

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
    public void Constructor_ThrowsWhenJobsIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            new TrackedJobsResult(expiredJids: [], jobs: null!);
        });
        Assert.Equal("Value cannot be null. (Parameter 'jobs')", exception.Message);
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
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            new TrackedJobsResult(expiredJids: null!, jobs: []);
        });
        Assert.Equal(
            "Value cannot be null. (Parameter 'expiredJids')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="TrackedJobsResult(Job[], string[])"/> should throw when
    /// expiredJids includes values that are null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenExpiredJidsIncludesNullEmptyOrOnlyWhitespaceValue()
    {
        foreach (var invalidJid in TestConstants.EmptyStringsWithNull)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                new TrackedJobsResult(expiredJids: [invalidJid!], jobs: []);
            });
            Assert.Equal(
                "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'expiredJids')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="TrackedJobsResult(Job[], string[])"/> should assigns
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