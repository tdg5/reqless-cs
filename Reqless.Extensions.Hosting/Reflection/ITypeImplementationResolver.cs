namespace Reqless.Extensions.Hosting.Reflection;

/// <summary>
/// Helper for resolving types that implement a given contract.
/// </summary>
public interface ITypeImplementationResolver
{
    /// <summary>
    /// Retrieve all types that implement the given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The contract type to search for.</param> <returns>An
    /// enumerable of types that implement the given contract.</returns>
    public Type[] GetAllImplementingTypes(Type type);
}
