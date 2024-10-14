using Reqless.Framework;

namespace Reqless.Tests.Fixtures.UnitOfWork;

/// <summary>
/// A generic implementation of <see cref="IUnitOfWork"/> that does nothing.
/// </summary>
/// <typeparam name="T">Any type.</typeparam>
public class GenericNoopUnitOfWork<T> : IUnitOfWork
{
    /// <inheritdoc/>
    public Task PerformAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
