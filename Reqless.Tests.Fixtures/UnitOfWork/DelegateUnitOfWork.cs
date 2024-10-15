using Reqless.Framework;

namespace Reqless.Tests.Fixtures.UnitOfWork;

/// <summary>
/// An implementation of <see cref="IUnitOfWork"/> that peforms an action
/// registered with the service provider.
/// </summary>
public class DelegateUnitOfWork : IUnitOfWork
{
    private readonly DelegateUnitOfWorkAction _action;

    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateUnitOfWork"/> class.
    /// </summary>
    /// <param name="action">The action to be performed.</param>
    /// <param name="serviceProvider">The service provider to be used when
    /// performing the action.</param>
    public DelegateUnitOfWork(
        DelegateUnitOfWorkAction action,
        IServiceProvider serviceProvider
    )
    {
        _action = action;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public Task PerformAsync(CancellationToken cancellationToken)
    {
        return _action.Action(_serviceProvider, cancellationToken);
    }
}
