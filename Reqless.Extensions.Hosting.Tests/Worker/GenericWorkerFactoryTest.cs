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
            () => new GenericWorkerFactory<NoopWorker>(null!),
            "workerNameProvider");
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
            () => subject.Create(null!), "serviceProvider");
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

    /// <summary>
    /// An implementation of <see cref="IWorker"/> that does nothing.
    /// </summary>
    internal class NoopWorker : IWorker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoopWorker"/> class.
        /// </summary>
        /// <param name="name">The name of the worker.</param>
        /// <param name="workerNameProvider">The worker name provider.</param>
        /// <remarks>
        /// Takes a workerNameProvider for testing dependency injection.
        /// </remarks>
        public NoopWorker(string name, IWorkerNameProvider workerNameProvider)
        {
            Name = name;
            WorkerNameProvider = workerNameProvider;
        }

        /// <summary>
        /// Gets the name of the worker.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the worker name provider.
        /// </summary>
        public IWorkerNameProvider WorkerNameProvider { get; }

        /// <summary>
        /// Executes the worker.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task ExecuteAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
