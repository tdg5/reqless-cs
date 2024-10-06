using Reqless.Extensions.Hosting.Worker;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Tests for <see cref="DefaultWorkerNameProvider"/>.
/// </summary>
public class DefaultWorkerNameProviderTest
{
    /// <summary>
    /// <see cref="DefaultWorkerNameProvider(string)"/> should throw when given
    /// a prefix that is empty or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGivenPrefixIsEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsEmptyOrWhitespace(
            (invalidPrefix) => new DefaultWorkerNameProvider(invalidPrefix),
            "prefix"
        );
    }

    /// <summary>
    /// <see cref="DefaultWorkerNameProvider(string)"/> should default the
    /// prefix to the expected value.
    /// </summary>
    [Fact]
    public void Constructor_DefaultsPrefixToExpectedValue()
    {
        var expectedPrefix = "ReqlessWorker";
        DefaultWorkerNameProvider subject = new(expectedPrefix);
        Assert.Equal($"{expectedPrefix}-1", subject.GetWorkerName());
    }

    /// <summary>
    /// <see cref="DefaultWorkerNameProvider(string)"/> should take the prefix
    /// value when given.
    /// </summary>
    [Fact]
    public void Constructor_TakesPrefixValueWhenGiven()
    {
        var expectedPrefix = "Prefix";
        DefaultWorkerNameProvider subject = new(expectedPrefix);
        Assert.Equal($"{expectedPrefix}-1", subject.GetWorkerName());
    }

    /// <summary>
    /// <see cref="DefaultWorkerNameProvider.GetWorkerName"/> should return the
    /// expected worker name on multiple invocations.
    /// </summary>
    [Fact]
    public void GetWorkerName_ReturnsExpectedWorkerName()
    {
        var expectedPrefix = "ReqlessWorker";
        var subject = new DefaultWorkerNameProvider();
        foreach (var index in Enumerable.Range(1, 20))
        {
            Assert.Equal($"{expectedPrefix}-{index}", subject.GetWorkerName());
        }
    }
}
