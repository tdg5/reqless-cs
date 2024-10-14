using Reqless.Framework;

namespace Reqless.Tests.Fixtures.UnitOfWork;

/// <summary>
/// An interface that extends <see cref="IUnitOfWork"/> with an extra method.
/// </summary>
public interface IUnitOFWorkWithExtras : IUnitOfWork
{
    /// <summary>
    /// A method that should do nothing.
    /// </summary>
    void Extras();
}
