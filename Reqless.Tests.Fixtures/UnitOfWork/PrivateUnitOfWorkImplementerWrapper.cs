using Reqless.Framework;

namespace Reqless.Tests.Fixtures.UnitOfWork;

/// <summary>
/// Elements defined in a namespace cannot be explicitly declared as private,
/// protected, protected internal, or private protected, so the class needs to
/// be wrapped in order to be declared private.
/// </summary>
internal static class PrivateUnitOfWorkImplementerWrapper
{
    /// <summary>
    /// An implementation of <see cref="IUnitOfWork"/> that does nothing.
    /// </summary>
    private class PrivateUnitOfWorkImplementer : IUnitOfWork
    {
        /// <inheritdoc/>
        public Task PerformAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
