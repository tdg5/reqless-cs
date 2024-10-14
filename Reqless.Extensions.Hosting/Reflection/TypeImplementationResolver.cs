using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Reqless.Extensions.Hosting.Reflection;

/// <summary>
/// A utility class for resolving types that implement a given contract.
/// </summary>
public class TypeImplementationResolver : ITypeImplementationResolver
{
    private readonly IReflectionGlobals _reflectionGlobals;

    /// <summary>
    /// Create a new instance of <see cref="TypeImplementationResolver"/>.
    /// </summary>
    /// <param name="reflectionGlobals">An <see cref="IReflectionGlobals"/>
    /// instance that should be used to access global/static reflection
    /// methods.</param>
    public TypeImplementationResolver(IReflectionGlobals? reflectionGlobals = null)
    {
        _reflectionGlobals = reflectionGlobals ?? new DefaultReflectionGlobals();
    }

    /// <summary>
    /// Get all types that implement the given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The contract to find implementing types for.</param>
    public Type[] GetAllImplementingTypes(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        return GetAssemblies()
            .SelectMany(
                assembly => TryGetTypes(assembly, out var types) ? types : []
            )
            .Where(candidateType =>
                !candidateType.IsInterface && !candidateType.IsAbstract
                && !candidateType.IsGenericTypeDefinition
                && AllBaseAndImplementingTypes(candidateType).Contains(type)
            )
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Get all base types of the given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type to discover the base types of.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Type"/> instances
    /// that are the base types of the given <paramref name="type"/>.</returns>
    internal static IEnumerable<Type> GetBaseTypes(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return EnumerateBaseTypes(type);
    }

    /// <remarks>
    /// This is separate from <see cref="GetBaseTypes"/> so that <see
    /// cref="GetBaseTypes"/> can validate arguments.
    /// </remarks>
    internal static IEnumerable<Type> EnumerateBaseTypes(Type type)
    {
        var currentType = type;
        while (currentType != null)
        {
            yield return currentType;
            currentType = currentType.GetTypeInfo().BaseType;
        }
    }

    /// <summary>
    /// Returns self and all base and implementing types of a given type, both
    /// open and closed generic (if any).
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to get base and implementing
    /// types for.</param>
    /// <returns>All base and implementing <see
    /// cref="Type">types</see>.</returns>
    internal static IEnumerable<Type> AllBaseAndImplementingTypes(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        return GetBaseTypes(type)
            .Concat(type.GetTypeInfo().GetInterfaces())
            .SelectMany(TypeAndOpenTypeIfGeneric)
            .Where(t => t != type && t != typeof(object));
    }

    internal static IEnumerable<Type> TypeAndOpenTypeIfGeneric(Type type)
    {
        yield return type;
        var typeInfo = type.GetTypeInfo();
        if (typeInfo.IsGenericType && !typeInfo.ContainsGenericParameters)
        {
            yield return type.GetGenericTypeDefinition();
        }
    }

    /// <summary>
    /// Get all assemblies that are referenced by the entry assembly in one way
    /// or another.
    /// </summary>
    /// <remarks>Protected and virtual to facilitate testing.</remarks>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Assembly"/>
    /// instances that are referenced by the entry assembly.</returns>
    internal protected virtual IEnumerable<Assembly> GetAssemblies()
    {
        List<Assembly> baseAssemblies = [];
        var entryAssembly = _reflectionGlobals.GetEntryAssembly()
            ?? throw new InvalidOperationException("Could not get the entry assembly.");

        baseAssemblies.Add(entryAssembly);

        var dependencyModel = _reflectionGlobals.LoadDependencyContext(entryAssembly)
            ?? throw new InvalidOperationException(
                "Could not load the dependency context for the entry assembly."
            );

        foreach (var runtimeLibrary in dependencyModel.RuntimeLibraries)
        {
            if (
                (
                    runtimeLibrary.Type.Equals("project")
                    || runtimeLibrary.RuntimeAssemblyGroups.Count > 0
                )
                && TryLoadAssemblyByName(
                    runtimeLibrary.Name,
                    out var runtimeLibraryAssembly
                )
            )
            {
                baseAssemblies.Add(runtimeLibraryAssembly);
            }
        }

        baseAssemblies.AddRange(_reflectionGlobals.CurrentAppDomain.GetAssemblies());
        return GetAssembliesCore(baseAssemblies);
    }

    /// <remarks>
    /// Implemented separately from <see cref="GetAssemblies"/> to facilitate
    /// throwing exceptions.
    /// </remarks>
    private IEnumerable<Assembly> GetAssembliesCore(List<Assembly> baseAssemblies)
    {
        Stack<Assembly> stack = new(baseAssemblies);
        HashSet<string> visited = [];

        do
        {
            var assembly = stack.Pop();

            yield return assembly;

            foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
            {
                string fullName = referencedAssemblyName.FullName;

                if (visited.Contains(fullName)) { continue; }

                visited.Add(fullName);

                if (TryLoadAssemblyByName(fullName, out var referencedAssembly))
                {
                    stack.Push(referencedAssembly);
                }
            }
        }
        while (stack.Count > 0);
    }

    /// <summary>
    /// Try to load an assembly by name, ignorning common exceptions that can be
    /// ignored for the purposes of this class.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to load.</param>
    /// <param name="assembly">The assembly that was loaded, or null if loading
    /// the assembly failed.</param>
    /// <returns>True when the assembly was successfully loaded, false
    /// otherwise.</returns>
    internal bool TryLoadAssemblyByName(
        string assemblyName,
        [MaybeNullWhen(false)] out Assembly assembly
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyName, nameof(assemblyName));

        try
        {
            assembly = _reflectionGlobals.LoadAssemblyByName(assemblyName);
            return true;
        }
        catch (Exception ex) when (
            ex is BadImageFormatException
            || ex is FileNotFoundException
            || ex is FileLoadException
            || ex is ReflectionTypeLoadException
        )
        {
            assembly = null;
            return false;
        }
    }

    /// <summary>
    /// Wrapper around <see cref="Assembly.GetTypes"/> that handles exceptions
    /// that sometimes come up and can be ignored for the purposes of this
    /// class.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> to get types from.</param>
    /// <param name="types">The types that were retrieved from the assembly, or
    /// null if loading types failed.</param>
    /// <returns>True when the types were successfully retrieved, false
    /// otherwise.</returns>
    internal static bool TryGetTypes(
        Assembly assembly,
        [MaybeNullWhen(false)] out Type[] types
    )
    {
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));

        try
        {
            types = assembly.GetTypes();
            return true;
        }
        catch (Exception ex) when (
            ex is ReflectionTypeLoadException
        )
        {
            types = null;
            return false;
        }
    }
}
