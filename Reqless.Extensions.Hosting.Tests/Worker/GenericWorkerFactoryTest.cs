using Microsoft.Extensions.DependencyInjection;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Tests for <see cref="GenericWorkerFactory{TWorker}"/>.
/// </summary>
public class GenericWorkerFactoryTest
{
    /// <summary>
    /// <see cref="GenericWorkerFactory{TWorker}"/> throws when the given
    /// worker name provider is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenWorkerNameProviderIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new GenericWorkerFactory<NoopWorker>(
                null!
            ),
            "workerNameProvider"
        );
    }

    /// <summary>
    /// <see cref="GenericWorkerFactory{TWorker}.Create(IServiceProvider)"/>
    /// should throw if the given service provider is null.
    /// </summary>
    [Fact]
    public void Create_ThrowsWhenServiceProviderIsNull()
    {
        var workerNameProvider = new DefaultWorkerNameProvider();
        var subject = new GenericWorkerFactory<NoopWorker>(workerNameProvider);
        Scenario.ThrowsWhenArgumentIsNull(
            () => subject.Create(null!),
            "serviceProvider"
        );
    }

    /// <summary>
    /// <see cref="GenericWorkerFactory{TWorker}.Create(IServiceProvider)"/>
    /// creates and returns a worker with the expected name using the given
    /// service provider.
    /// </summary>
    [Fact]
    public void Create_ReturnsWorkerWithExpectedNameUsingServiceProvider()
    {
        var workerNamePrefix = "Worker";
        var workerNameProvider = new DefaultWorkerNameProvider(workerNamePrefix);
        var subject = new GenericWorkerFactory<NoopWorker>(workerNameProvider);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IWorkerNameProvider>(workerNameProvider);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var worker = subject.Create(serviceProvider);
        Assert.Equal($"{workerNamePrefix}-1", worker.Name);
        Assert.IsAssignableFrom<NoopWorker>(worker);
        Assert.Same(workerNameProvider, ((NoopWorker)worker).WorkerNameProvider);
    }

    internal class NoopWorker : IWorker
    {
        public string Name { get; }

        public IWorkerNameProvider WorkerNameProvider { get; }

        // Takes a workerNameProvider for testing dependency injection.
        public NoopWorker(string name, IWorkerNameProvider workerNameProvider)
        {
            Name = name;
            WorkerNameProvider = workerNameProvider;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
