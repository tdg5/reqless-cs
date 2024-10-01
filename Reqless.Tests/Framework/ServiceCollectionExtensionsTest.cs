using Microsoft.Extensions.DependencyInjection;
using Moq;
using Reqless.Client;
using Reqless.Framework;

namespace Reqless.Tests.Framework;

/// <summary>
/// Unit tests for the <see cref="ServiceCollectionExtensions"/> class.
/// </summary>
public class ServiceCollectionExtensionsTest
{
    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessServices"/> should add
    /// an <see cref="IReqlessClient"/> service if none is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_AddsIReqlessClientIfNoneRegistered()
    {
        ServiceCollection services = new();
        var expectedClient = Mock.Of<IReqlessClient>();
        var mockFactory = new Mock<IReqlessClientFactory>();
        mockFactory.Setup(_ => _.Create()).Returns(expectedClient);
        services.AddSingleton<IReqlessClientFactory>(mockFactory.Object);
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IReqlessClient subject = provider.GetRequiredService<IReqlessClient>();
        Assert.Same(expectedClient, subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessServices"/> doesn't
    /// override <see cref="IReqlessClient"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_DoesNotOverrideRegisteredIReqlessClient()
    {
        ServiceCollection services = new();
        var mock = Mock.Of<IReqlessClient>();
        services.AddSingleton<IReqlessClient>(mock);
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IReqlessClient subject = provider.GetRequiredService<IReqlessClient>();
        Assert.Same(mock, subject);
    }
}
