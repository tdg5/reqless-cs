using Reqless.Framework;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Contract for classes that support resolving classes that implement the <see
/// cref="IUnitOfWork"/> interface from full class names.
/// </summary>
public interface IUnitOfWorkResolver
{
    /// <summary>
    /// Resolve a type that implements <see cref="IUnitOfWork"/> from a string
    /// containing the type's full name.
    /// </summary>
    /// <param name="typeName">The full name of the type.</param>
    /// <returns>The type if it could be resolved and implements <see
    /// cref="IUnitOfWork"/>, otherwise null.</returns>
    Type? Resolve(string typeName);
}
