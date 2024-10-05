using Microsoft.Extensions.DependencyInjection;
using Moq;
using Reqless.Client;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Framework;

namespace Reqless.Worker.Tests;

/// <summary>
/// Unit tests for the <see cref="ServiceCollectionExtensions"/> class.
/// </summary>
public class ServiceCollectionExtensionsTest
{
    WorkerSettings ExampleSettings = new(
        connectionString: "Server=localhost;",
        queueIdentifiers: ["queue"],
        workerCount: 1
    );

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessWorkerServices"/> should add
    /// an <see cref="IUnitOfWorkActivator"/> service if none is registered.
    /// </summary>
    [Fact]
    public void AddReqlessWorkerServices_AddsIUnitOfWorkActivatorIfNoneRegistered()
    {
        ServiceCollection services = new();
        services.AddReqlessWorkerServices(ExampleSettings);
        var provider = services.BuildServiceProvider();
        IUnitOfWorkActivator subject = provider.GetRequiredService<IUnitOfWorkActivator>();
        Assert.IsType<DefaultUnitOfWorkActivator>(subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessWorkerServices"/> doesn't
    /// override <see cref="IUnitOfWorkActivator"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessWorkerServices_DoesNotOverrideRegisteredIUnitOfWorkActivator()
    {
        ServiceCollection services = new();
        var mock = Mock.Of<IUnitOfWorkActivator>();
        services.AddSingleton<IUnitOfWorkActivator>(mock);
        services.AddReqlessWorkerServices(ExampleSettings);
        var provider = services.BuildServiceProvider();
        IUnitOfWorkActivator subject = provider.GetRequiredService<IUnitOfWorkActivator>();
        Assert.Same(mock, subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessWorkerServices"/> should add
    /// an <see cref="IUnitOfWorkResolver"/> service if none is registered.
    /// </summary>
    [Fact]
    public void AddReqlessWorkerServices_AddsIUnitOfWorkResolverIfNoneRegistered()
    {
        ServiceCollection services = new();
        services.AddReqlessWorkerServices(ExampleSettings);
        var provider = services.BuildServiceProvider();
        IUnitOfWorkResolver subject = provider.GetRequiredService<IUnitOfWorkResolver>();
        Assert.IsType<DefaultUnitOfWorkResolver>(subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessWorkerServices"/> doesn't
    /// override <see cref="IUnitOfWorkResolver"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessWorkerServices_DoesNotOverrideRegisteredIUnitOfWorkResolver()
    {
        ServiceCollection services = new();
        var mock = Mock.Of<IUnitOfWorkResolver>();
        services.AddSingleton<IUnitOfWorkResolver>(mock);
        services.AddReqlessWorkerServices(ExampleSettings);
        var provider = services.BuildServiceProvider();
        IUnitOfWorkResolver subject = provider.GetRequiredService<IUnitOfWorkResolver>();
        Assert.Same(mock, subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessWorkerServices"/> should add
    /// an <see cref="IJobContextFactory"/> service if none is registered.
    /// </summary>
    [Fact]
    public void AddReqlessWorkerServices_AddsIJobContextFactoryIfNoneRegistered()
    {
        ServiceCollection services = new();
        services.AddReqlessWorkerServices(ExampleSettings);
        var provider = services.BuildServiceProvider();
        IJobContextFactory subject = provider.GetRequiredService<IJobContextFactory>();
        Assert.IsType<DefaultJobContextFactory>(subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessWorkerServices"/> doesn't
    /// override <see cref="IJobContextFactory"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessWorkerServices_DoesNotOverrideRegisteredIJobContextFactory()
    {
        ServiceCollection services = new();
        var mock = Mock.Of<IJobContextFactory>();
        services.AddSingleton<IJobContextFactory>(mock);
        services.AddReqlessWorkerServices(ExampleSettings);
        var provider = services.BuildServiceProvider();
        IJobContextFactory subject = provider.GetRequiredService<IJobContextFactory>();
        Assert.Same(mock, subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessWorkerServices"/> should add
    /// an <see cref="IJobContextAccessor"/> service if none is registered.
    /// </summary>
    [Fact]
    public void AddReqlessWorkerServices_AddsIJobContextAccessorIfNoneRegistered()
    {
        ServiceCollection services = new();
        services.AddReqlessWorkerServices(ExampleSettings);
        var provider = services.BuildServiceProvider();
        IJobContextAccessor subject = provider.GetRequiredService<IJobContextAccessor>();
        Assert.IsType<DefaultJobContextAccessor>(subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessWorkerServices"/> doesn't
    /// override <see cref="IJobContextAccessor"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessWorkerServices_DoesNotOverrideRegisteredIJobContextAccessor()
    {
        ServiceCollection services = new();
        var mock = Mock.Of<IJobContextAccessor>();
        services.AddSingleton<IJobContextAccessor>(mock);
        services.AddReqlessWorkerServices(ExampleSettings);
        var provider = services.BuildServiceProvider();
        IJobContextAccessor subject = provider.GetRequiredService<IJobContextAccessor>();
        Assert.Same(mock, subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessWorkerServices"/>
    /// should add an <see cref="IReqlessClient"/> service if none is
    /// registered.
    /// </summary>
    [Fact]
    public void AddReqlessWorkerServices_AddsIReqlessClientIfNoneRegistered()
    {
        ServiceCollection services = new();
        var expectedClient = Mock.Of<IReqlessClient>();
        var mockFactory = new Mock<IReqlessClientFactory>();
        mockFactory.Setup(_ => _.Create()).Returns(expectedClient);
        services.AddSingleton<IReqlessClientFactory>(mockFactory.Object);
        services.AddReqlessWorkerServices(ExampleSettings);
        var provider = services.BuildServiceProvider();
        IReqlessClient subject = provider.GetRequiredService<IReqlessClient>();
        Assert.Same(expectedClient, subject);
    }

    /// <summary>
    /// <see cref="ServiceCollectionExtensions.AddReqlessWorkerServices"/> doesn't
    /// override <see cref="IReqlessClient"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessWorkerServices_DoesNotOverrideRegisteredIReqlessClient()
    {
        ServiceCollection services = new();
        var mock = Mock.Of<IReqlessClient>();
        services.AddSingleton<IReqlessClient>(mock);
        services.AddReqlessWorkerServices(ExampleSettings);
        var provider = services.BuildServiceProvider();
        IReqlessClient subject = provider.GetRequiredService<IReqlessClient>();
        Assert.Same(mock, subject);
    }
}
