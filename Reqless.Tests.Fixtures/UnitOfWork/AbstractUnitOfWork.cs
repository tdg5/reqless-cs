using Reqless.Framework;

namespace Reqless.Tests.Fixtures.UnitOfWork;

/// <summary>
/// A type that includes the <see cref="IUnitOfWork"/> interface, but does not
/// actually implement it.
/// </summary>
public abstract class AbstractUnitOfWork : IUnitOfWork
{
    /// <inheritdoc/>
    public abstract Task PerformAsync(CancellationToken cancellationToken);
}
