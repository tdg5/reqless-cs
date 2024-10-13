using Reqless.Framework;

namespace Reqless.Tests.Fixtures.UnitOfWork;

/// <summary>
/// A class that does not implement <see cref="IUnitOfWork"/> directly, but
/// instead, inherits from a class that does.
/// </summary>
public class UnitOfWorkImplementerSubclass : UnitOfWorkImplementer
{
}
