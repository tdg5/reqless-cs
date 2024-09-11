namespace Reqless.Framework;

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
    /// <param name="provider">An <see cref="IServiceProvider"/> instance to use
    /// for service resolution.</param>
    /// <param name="instanceType">The type of instance that should be
    /// created.</param>
    /// <returns>An <see cref="IUnitOfWork"/> instance.</returns>
    IUnitOfWork CreateInstance(
        IServiceProvider provider,
        Type instanceType
    );
}