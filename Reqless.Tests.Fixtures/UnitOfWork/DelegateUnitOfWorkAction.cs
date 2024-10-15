namespace Reqless.Tests.Fixtures.UnitOfWork;

/// <summary>
/// A wrapper around an action than can be injected into a <see
/// cref="DelegateUnitOfWork"/> through a service provider.
/// </summary>
public class DelegateUnitOfWorkAction
{
    /// <summary>
    /// The action to be performed.
    /// </summary>
    public Func<IServiceProvider, CancellationToken, Task> Action { get; }

    /// <summary>
    /// Create an instance of <see cref="DelegateUnitOfWorkAction"/>.
    /// </summary>
    /// <param name="action">The action to be performed.</param>
    public DelegateUnitOfWorkAction(
        Func<IServiceProvider, CancellationToken, Task> action
    )
    {
        Action = action;
    }
}
