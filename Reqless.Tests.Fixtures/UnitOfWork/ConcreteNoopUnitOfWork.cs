using Reqless.Framework;

namespace Reqless.Tests.Fixtures.UnitOfWork;

/// <summary>
/// A class that does not implement <see cref="IUnitOfWork"/> directly, but
/// instead, inherits from a class that does.
/// </summary>
public class ConcreteNoopUnitOfWork : AbstractUnitOfWork
{
    /// <inheritdoc/>
    public override Task PerformAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
