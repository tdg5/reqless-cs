using Moq;
using Reqless.Client;
using Reqless.Client.Models;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Test class for <see cref="DefaultQueueIdentifierResolver"/>.
/// </summary>
public class DefaultQueueIdentifierResolverTest
{
    /// <summary>
    /// <see cref="DefaultQueueIdentifierResolver(IReqlessClient)"/> should
    /// throw when the given reqless client is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGivenReqlessClientIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new DefaultQueueIdentifierResolver(null!),
            "reqlessClient");
    }

    /// <summary>
    /// <see cref="DefaultQueueIdentifierResolver.ResolveQueueNamesAsync"/> should
    /// throw when any given queue name is null.
    /// </summary>
    /// <returns>A Task denoting the completion of the test.</returns>
    [Fact]
    public async Task ResolveQueueNamesAsync_ThrowsWhenAnyGivenQueueNameIsNullOrEmptyOrWhiteSpace()
    {
        DefaultQueueIdentifierResolver subject = new(Mock.Of<IReqlessClient>());
        await Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespaceAsync(
            invalidValue => subject.ResolveQueueNamesAsync(invalidValue!),
            "queueIdentifiers");
    }

    /// <summary>
    /// <see cref="DefaultQueueIdentifierResolver.ResolveQueueNamesAsync"/> should
    /// return the resolved queue names.
    /// </summary>
    /// <returns>A Task denoting the completion of the test.</returns>
    [Fact]
    public async Task ResolveQueueNamesAsync_ReturnsResolvedQueueNames()
    {
        var reqlessClientMock = new Mock<IReqlessClient>();
        var expectedIdentifier = "@identifier";
        reqlessClientMock
            .Setup(_ => _.GetAllQueuePriorityPatternsAsync())
            .ReturnsAsync([
                new QueuePriorityPattern(["a"]),
                new QueuePriorityPattern(["b"]),
                new QueuePriorityPattern(["c"]),
            ]);
        reqlessClientMock
            .Setup(_ => _.GetAllQueueIdentifierPatternsAsync())
            .ReturnsAsync(new Dictionary<string, List<string>>()
            {
                ["identifier"] = ["c", "b", "a"],
            });
        DefaultQueueIdentifierResolver subject = new(reqlessClientMock.Object);
        List<string> expectedQueueNames = ["a", "b", "c"];
        var queueNames = await subject.ResolveQueueNamesAsync(expectedIdentifier);
        Assert.Equal(expectedQueueNames, queueNames);
    }
}
