using Microsoft.Extensions.DependencyInjection;
using Reqless.Client;
using Reqless.Client.Models;
using Reqless.Framework;

namespace Reqless.Tests.Framework;

/// <summary>
/// Unit tests for the <see cref="ServiceCollectionExtensions"/> class.
/// </summary>
public class ServiceCollectionExtensionsTest
{
    /// <summary>
    /// cref="ServiceCollectionExtensions.AddReqlessServices"/> should add an
    /// <see cref="IUnitOfWorkActivator"/> service if none is registered.
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
    /// cref="ServiceCollectionExtensions.AddReqlessServices"/> doesn't override
    /// <see cref="IUnitOfWorkActivator"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_DoesNotOverrideRegisteredIUnitOfWorkActivator()
    {
        ServiceCollection services = new();
        services.AddSingleton<IUnitOfWorkActivator, NoopUnitOfWorkActivator>();
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IUnitOfWorkActivator subject = provider.GetRequiredService<IUnitOfWorkActivator>();
        Assert.IsType<NoopUnitOfWorkActivator>(subject);
    }

    /// <summary>
    /// cref="ServiceCollectionExtensions.AddReqlessServices"/> should add an
    /// <see cref="IUnitOfWorkResolver"/> service if none is registered.
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
    /// cref="ServiceCollectionExtensions.AddReqlessServices"/> doesn't override
    /// <see cref="IUnitOfWorkResolver"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_DoesNotOverrideRegisteredIUnitOfWorkResolver()
    {
        ServiceCollection services = new();
        services.AddSingleton<IUnitOfWorkResolver, NoopUnitOfWorkResolver>();
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IUnitOfWorkResolver subject = provider.GetRequiredService<IUnitOfWorkResolver>();
        Assert.IsType<NoopUnitOfWorkResolver>(subject);
    }

    /// <summary>
    /// cref="ServiceCollectionExtensions.AddReqlessServices"/> should add an
    /// <see cref="IJobContextFactory"/> service if none is registered.
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
    /// cref="ServiceCollectionExtensions.AddReqlessServices"/> doesn't override
    /// <see cref="IJobContextFactory"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_DoesNotOverrideRegisteredIJobContextFactory()
    {
        ServiceCollection services = new();
        services.AddSingleton<IJobContextFactory, NoopJobContextFactory>();
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IJobContextFactory subject = provider.GetRequiredService<IJobContextFactory>();
        Assert.IsType<NoopJobContextFactory>(subject);
    }

    /// <summary>
    /// cref="ServiceCollectionExtensions.AddReqlessServices"/> should add an
    /// <see cref="IJobContextAccessor"/> service if none is registered.
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
    /// cref="ServiceCollectionExtensions.AddReqlessServices"/> doesn't override
    /// <see cref="IJobContextAccessor"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_DoesNotOverrideRegisteredIJobContextAccessor()
    {
        ServiceCollection services = new();
        services.AddSingleton<IJobContextAccessor, NoopJobContextAccessor>();
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IJobContextAccessor subject = provider.GetRequiredService<IJobContextAccessor>();
        Assert.IsType<NoopJobContextAccessor>(subject);
    }

    /// <summary>
    /// cref="ServiceCollectionExtensions.AddReqlessServices"/> should add an
    /// <see cref="IReqlessClientAccessor"/> service if none is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_AddsIReqlessClientAccessorIfNoneRegistered()
    {
        ServiceCollection services = new();
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IReqlessClientAccessor subject = provider.GetRequiredService<IReqlessClientAccessor>();
        Assert.IsType<DefaultReqlessClientAccessor>(subject);
    }

    /// <summary>
    /// cref="ServiceCollectionExtensions.AddReqlessServices"/> doesn't override
    /// <see cref="IReqlessClientAccessor"/> service if one is registered.
    /// </summary>
    [Fact]
    public void AddReqlessServices_DoesNotOverrideRegisteredIReqlessClientAccessor()
    {
        ServiceCollection services = new();
        services.AddSingleton<IReqlessClientAccessor, NoopReqlessClientAccessor>();
        services.AddReqlessServices();
        var provider = services.BuildServiceProvider();
        IReqlessClientAccessor subject = provider.GetRequiredService<IReqlessClientAccessor>();
        Assert.IsType<NoopReqlessClientAccessor>(subject);
    }

    class NoopJobContextAccessor : IJobContextAccessor
    {
        public IJobContext? Value
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }

    class NoopJobContextFactory : IJobContextFactory
    {
        public IJobContext Create(Job _) => throw new NotImplementedException();
    }

    class NoopReqlessClientAccessor : IReqlessClientAccessor
    {
        public IReqlessClient? Value
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }

    class NoopUnitOfWorkActivator : IUnitOfWorkActivator
    {
        public IUnitOfWork CreateInstance(Type instanceType)
        {
            throw new NotImplementedException();
        }
    }

    class NoopUnitOfWorkResolver : IUnitOfWorkResolver
    {
        public Type Resolve(string _) => throw new NotImplementedException();
    }
}
