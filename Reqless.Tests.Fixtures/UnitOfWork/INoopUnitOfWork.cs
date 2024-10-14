using Reqless.Framework;

namespace Reqless.Tests.Fixtures.UnitOfWork;

/// <summary>
/// An interface that extends <see cref="IUnitOfWork"/> with an extra method.
/// </summary>
public interface INoopUnitOfWork : IUnitOfWork
{
    /// <inheritdoc/>
    Task IUnitOfWork.PerformAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
