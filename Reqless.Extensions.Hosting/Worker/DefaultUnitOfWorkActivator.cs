using Microsoft.Extensions.DependencyInjection;
using Reqless.Framework;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Default <see cref="IUnitOfWorkActivator"/> for creating instances of classes
/// that implement the <see cref="IUnitOfWork"/> interface.
/// </summary>
public class DefaultUnitOfWorkActivator : IUnitOfWorkActivator
{
    /// <inheritdoc />
    public IUnitOfWork CreateInstance(IServiceProvider serviceProvider, Type instanceType)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        ArgumentNullException.ThrowIfNull(instanceType, nameof(instanceType));

        if (!typeof(IUnitOfWork).IsAssignableFrom(instanceType))
        {
            throw new InvalidOperationException(
                $"{instanceType.FullName} does not implement"
                    + $" {typeof(IUnitOfWork).FullName}.");
        }

        return (IUnitOfWork)ActivatorUtilities.CreateInstance(
            serviceProvider,
            instanceType);
    }
}
