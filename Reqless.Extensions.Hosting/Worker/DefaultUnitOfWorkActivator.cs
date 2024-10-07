using Microsoft.Extensions.DependencyInjection;
using Reqless.Framework;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Default <see cref="IUnitOfWorkActivator"/> for creating instances of classes
/// that implement the <see cref="IUnitOfWork"/> interface.
/// </summary>
public class DefaultUnitOfWorkActivator : IUnitOfWorkActivator
{
    // Use a class attribute to identify the job data class.
    // Maybe also use a parameter attribute to identify the job data parameter in case it is ambiguous?
    //   - Most likely to be ambiguous when it's a string.
    //   - Parameter name of data could be a hint too.
    // Use reflection to find an optional parameter of some kind of abstract job class?

    /// <inheritdoc />
    public IUnitOfWork CreateInstance(IServiceProvider serviceProvider, Type instanceType)
    {
        if (
            ActivatorUtilities.CreateInstance(serviceProvider, instanceType)
            is IUnitOfWork unitOfWork
        )
        {
            return unitOfWork;
        }

        throw new InvalidOperationException(
            $"Could not create an instance of the unit of work class '{instanceType.FullName}'."
        );
    }
}
