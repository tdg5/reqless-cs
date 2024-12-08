using Moq;
using Reqless.Client;
using Reqless.Framework;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Framework;

/// <summary>
/// Tests for <see cref="DelegatingReqlessClientFactory"/>.
/// </summary>
public class DelegatingReqlessClientFactoryTest
{
    /// <summary>
    /// <see cref="DelegatingReqlessClientFactory(Func{IReqlessClient})"/>
    /// should throw when the client factory argument is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenClientFactoryIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new DelegatingReqlessClientFactory(null!),
            "clientFactory");
    }

    /// <summary>
    /// <see cref="DelegatingReqlessClientFactory.Create"/> should call the
    /// delegate and return the result.
    /// </summary>
    [Fact]
    public void Create_CallsTheDelegateAndReturnsTheResult()
    {
        var client = Mock.Of<IReqlessClient>();
        IReqlessClient Factory() => client;
        DelegatingReqlessClientFactory subject = new(Factory);
        var actualClient = subject.Create();
        Assert.Same(client, actualClient);
    }
}
