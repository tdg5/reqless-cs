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
    /// an <see cref="IUnitOfWorkActivator"/> service if none is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_AddsIUnitOfWorkActivatorIfNoneRegistered()
    {
        ServiceCollection services = new();
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IUnitOfWorkActivator subject = provider.GetRequiredService<IUnitOfWorkActivator>();
        Assert.IsType<DefaultUnitOfWorkActivator>(subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessServices"/> doesn't
    /// override <see cref="IUnitOfWorkActivator"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_DoesNotOverrideRegisteredIUnitOfWorkActivator()
    {
        ServiceCollection services = new();
        var mock = Mock.Of<IUnitOfWorkActivator>();
        services.AddSingleton<IUnitOfWorkActivator>(mock);
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IUnitOfWorkActivator subject = provider.GetRequiredService<IUnitOfWorkActivator>();
        Assert.Same(mock, subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessServices"/> should add
    /// an <see cref="IUnitOfWorkResolver"/> service if none is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_AddsIUnitOfWorkResolverIfNoneRegistered()
    {
        ServiceCollection services = new();
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IUnitOfWorkResolver subject = provider.GetRequiredService<IUnitOfWorkResolver>();
        Assert.IsType<DefaultUnitOfWorkResolver>(subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessServices"/> doesn't
    /// override <see cref="IUnitOfWorkResolver"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_DoesNotOverrideRegisteredIUnitOfWorkResolver()
    {
        ServiceCollection services = new();
        var mock = Mock.Of<IUnitOfWorkResolver>();
        services.AddSingleton<IUnitOfWorkResolver>(mock);
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IUnitOfWorkResolver subject = provider.GetRequiredService<IUnitOfWorkResolver>();
        Assert.Same(mock, subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessServices"/> should add
    /// an <see cref="IJobContextFactory"/> service if none is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_AddsIJobContextFactoryIfNoneRegistered()
    {
        ServiceCollection services = new();
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IJobContextFactory subject = provider.GetRequiredService<IJobContextFactory>();
        Assert.IsType<DefaultJobContextFactory>(subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessServices"/> doesn't
    /// override <see cref="IJobContextFactory"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_DoesNotOverrideRegisteredIJobContextFactory()
    {
        ServiceCollection services = new();
        var mock = Mock.Of<IJobContextFactory>();
        services.AddSingleton<IJobContextFactory>(mock);
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IJobContextFactory subject = provider.GetRequiredService<IJobContextFactory>();
        Assert.Same(mock, subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessServices"/> should add
    /// an <see cref="IJobContextAccessor"/> service if none is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_AddsIJobContextAccessorIfNoneRegistered()
    {
        ServiceCollection services = new();
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IJobContextAccessor subject = provider.GetRequiredService<IJobContextAccessor>();
        Assert.IsType<DefaultJobContextAccessor>(subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessServices"/> doesn't
    /// override <see cref="IJobContextAccessor"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_DoesNotOverrideRegisteredIJobContextAccessor()
    {
        ServiceCollection services = new();
        var mock = Mock.Of<IJobContextAccessor>();
        services.AddSingleton<IJobContextAccessor>(mock);
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IJobContextAccessor subject = provider.GetRequiredService<IJobContextAccessor>();
        Assert.Same(mock, subject);
    }

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
