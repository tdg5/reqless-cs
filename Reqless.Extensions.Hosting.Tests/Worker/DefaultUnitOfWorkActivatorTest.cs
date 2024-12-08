using Microsoft.Extensions.DependencyInjection;
using Moq;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Framework;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Tests for <see cref="DefaultUnitOfWorkActivator"/>.
/// </summary>
public class DefaultUnitOfWorkActivatorTest
{
    /// <summary>
    /// <see cref="DefaultUnitOfWorkActivator.CreateInstance"/> should throw if
    /// the given service provider is null.
    /// </summary>
    [Fact]
    public void CreateInstance_ThrowsIfTheGivenServiceProviderIsNull()
    {
        DefaultUnitOfWorkActivator subject = new();
        Scenario.ThrowsWhenArgumentIsNull(
            () => subject.CreateInstance(null!, typeof(Type)),
            "serviceProvider");
    }

    /// <summary>
    /// <see cref="DefaultUnitOfWorkActivator.CreateInstance"/> should throw if
    /// the given instance type is null.
    /// </summary>
    [Fact]
    public void CreateInstance_ThrowsIfAnInstanceCannotBeCreated()
    {
        var serviceProvider = Mock.Of<IServiceProvider>();
        DefaultUnitOfWorkActivator subject = new();
        Scenario.ThrowsWhenArgumentIsNull(
            () => subject.CreateInstance(serviceProvider, null!),
            "instanceType");
    }

    /// <summary>
    /// <see cref="DefaultUnitOfWorkActivator.CreateInstance"/> should throw if
    /// the created instance is not a unit of work.
    /// </summary>
    [Fact]
    public void CreateInstance_ThrowsIfTheGivenTypeDoesNotImplementIUnitOfWork()
    {
        ServiceCollection services = new();
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        DefaultUnitOfWorkActivator subject = new();
        var exception = Assert.Throws<InvalidOperationException>(
            () => subject.CreateInstance(serviceProvider, typeof(NotUnitOfWork)));
        Assert.Equal(
            $"{typeof(NotUnitOfWork).FullName} does not implement {typeof(IUnitOfWork).FullName}.",
            exception.Message);
    }

    /// <summary>
    /// <see cref="DefaultUnitOfWorkActivator.CreateInstance"/> should return an
    /// instance of the given type.
    /// </summary>
    [Fact]
    public void CreateInstance_ReturnsAnInstanceOfTheGivenType()
    {
        ServiceCollection services = new();
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        DefaultUnitOfWorkActivator subject = new();
        var instance = subject.CreateInstance(serviceProvider, typeof(UnitOfWork));
        Assert.IsType<UnitOfWork>(instance);
    }

    private class NotUnitOfWork
    {
    }

    private class UnitOfWork : IUnitOfWork
    {
        public Task PerformAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
