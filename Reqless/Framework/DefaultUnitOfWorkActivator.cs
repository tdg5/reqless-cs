using Microsoft.Extensions.DependencyInjection;

namespace Reqless.Framework;

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

    private readonly IServiceProvider _provider;

    /// <summary>
    /// Create an instance of <see cref="DefaultUnitOfWorkActivator"/>.
    /// </summary>
    /// <param name="provider">The <see cref="IUnitOfWorkActivator"/> instance
    /// to use when resolving dependencies.</param>
    public DefaultUnitOfWorkActivator(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public IUnitOfWork CreateInstance(Type instanceType)
    {
        // var attribute = instanceType.GetCustomAttribute<JobDataTypeAttribute>();
        // if (attribute == null)
        // {
        //     throw new InvalidOperationException("CollaboratorAttribute not found on class.");
        // }
        if (ActivatorUtilities.CreateInstance(_provider, instanceType) is not IUnitOfWork unitOfWork)
        {
            throw new InvalidOperationException(
                $"Could not create an instance of the unit of work class '{instanceType.FullName}'."
            );
        }
        return unitOfWork!;
    }
}