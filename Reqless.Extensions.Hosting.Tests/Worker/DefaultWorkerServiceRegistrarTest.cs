using Microsoft.Extensions.DependencyInjection;
using Reqless.Extensions.Hosting.Worker;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Tests for <see cref="DefaultWorkerServiceRegistrar"/>.
/// </summary>
public class DefaultWorkerServiceRegistrarTest
{
    /// <summary>
    /// <see
    /// cref="GenericWorkerServiceRegistrar{DefaultWorkerService}.RegisterWorkers"/>
    /// should register the expected number of workers.
    /// </summary>
    [Fact]
    public void RegisterWorkers_RegistersTheExpectedNumberOfWorkers()
    {
        DefaultWorkerServiceRegistrar subject = new();
        foreach (int count in Enumerable.Range(0, 5))
        {
            var serviceCollection = new ServiceCollection();
            var returnValue = subject.RegisterWorkers(serviceCollection, count);
            Assert.Same(serviceCollection, returnValue);
            var serviceDescriptors = new ServiceDescriptor[serviceCollection.Count];
            serviceCollection.CopyTo(serviceDescriptors, 0);
            foreach (var serviceDescriptor in serviceDescriptors)
            {
                Assert.Equal(
                    typeof(DefaultWorkerService),
                    serviceDescriptor.ImplementationType
                );
            }
        }
    }
}
