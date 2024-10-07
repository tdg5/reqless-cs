using Moq;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Framework;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Tests for <see cref="DefaultQueueNameProvider"/>.
/// </summary>
public class DefaultQueueNameProviderTest
{
    /// <summary>
    /// <see cref="DefaultQueueNameProvider(IQueueIdentifierResolver,
    /// IWorkerSettings)"/> throws when the queue identifier resolver argument
    /// is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenQueueIdentifierResolverIsNull()
    {
        WorkerSettings settings = new(
            queueIdentifiers: ["some-queue"],
            workerCount: 1
        );

        Scenario.ThrowsWhenArgumentIsNull(
            () => new DefaultQueueNameProvider(null!, settings),
            "queueIdentifierResolver"
        );
    }

    /// <summary>
    /// <see cref="DefaultQueueNameProvider(IQueueIdentifierResolver,
    /// IWorkerSettings)"/> throws when the settings argument is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenSettingsIsNull()
    {
        var queueIdentifierResolver = Mock.Of<IQueueIdentifierResolver>();

        Scenario.ThrowsWhenArgumentIsNull(
            () => new DefaultQueueNameProvider(queueIdentifierResolver, null!),
            "settings"
        );
    }

    /// <summary>
    /// <see cref="DefaultQueueNameProvider.GetQueueNamesAsync()"/> invokes the
    /// queue resolver with the expected queue identifiers.
    /// </summary>
    [Fact]
    public async Task GetQueueNamesAsync_InvokesTheResolverWithTheExpectedQueueIdentifiers()
    {
        string[] expectedQueueIdentifiers = ["some-queue"];
        WorkerSettings settings = new(
            queueIdentifiers: expectedQueueIdentifiers,
            workerCount: 1
        );
        List<string> expectedQueueNames = ["final-queue"];
        var mock = new Mock<IQueueIdentifierResolver>();
        mock.Setup(_ => _.ResolveQueueNamesAsync(expectedQueueIdentifiers))
            .ReturnsAsync(expectedQueueNames)
            .Verifiable();
        var queueIdentifierResolver = mock.Object;
        DefaultQueueNameProvider subject = new(queueIdentifierResolver, settings);
        var result = await subject.GetQueueNamesAsync();
        Assert.Equal(expectedQueueNames, result);
        mock.Verify();
        mock.VerifyNoOtherCalls();
    }
}
