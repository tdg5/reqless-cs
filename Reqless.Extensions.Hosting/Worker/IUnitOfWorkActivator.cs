using Reqless.Framework;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Contract for classes that support activating classes that implement the <see
/// cref="IUnitOfWork"/> interface.
/// </summary>
public interface IUnitOfWorkActivator
{
    /// <summary>
    /// Create an instance of the given class that implements <see
    /// cref="IUnitOfWork"/>.
    /// </summary>
    /// <param name="serviceProvider">An <see cref="IServiceProvider"/> instance
    /// that can be utilized when creating unit of work instances.</param>
    /// <param name="instanceType">The type of instance that should be
    /// created.</param>
    /// <returns>An <see cref="IUnitOfWork"/> instance.</returns>
    IUnitOfWork CreateInstance(
        IServiceProvider serviceProvider,
        Type instanceType
    );
}
