using Reqless.Framework;

namespace Reqless.Tests.Fixtures.UnitOfWork;

/// <summary>
/// An implementation of <see cref="IUnitOfWork"/> that does nothing.
/// </summary>
public class UnitOfWorkImplementer : IUnitOfWork
{
    /// <inheritdoc/>
    public Task PerformAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
