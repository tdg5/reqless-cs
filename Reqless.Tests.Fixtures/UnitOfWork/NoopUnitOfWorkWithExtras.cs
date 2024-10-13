using Reqless.Framework;

namespace Reqless.Tests.Fixtures.UnitOfWork;

/// <summary>
/// An interface that extends <see cref="IUnitOfWork"/> with an extra method.
/// </summary>
public class NoopUnitOfWorkWithExtras : IUnitOFWorkWithExtras
{
    /// <inheritdoc/>
    public void Extras()
    {
    }

    /// <inheritdoc/>
    public Task PerformAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
