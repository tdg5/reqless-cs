namespace Reqless.Framework;

/// <summary>
/// Contract for classes that support resolving classes that implement the <see
/// cref="IUnitOfWork"/> interface from full class names.
/// </summary>
public interface IUnitOfWorkResolver
{
    /// <summary>
    /// Resolve a class that implements <see cref="IUnitOfWork"/> from a string
    /// containing the class's full name.
    /// </summary>
    /// <param name="fullName">The full name of the class.</param>
    /// <returns>The class if it could be resolved, otherwise null.</returns>
    Type? Resolve(string fullName);
}