using Microsoft.Extensions.DependencyModel;
using Reqless.Framework;
using System.Reflection;

namespace Reqless.Worker;

/// <summary>
/// Concrete implmentation of <see cref="IUnitOfWorkResolver"/> that uses
/// reflection to resolve types.
/// </summary>
public class DefaultUnitOfWorkResolver : IUnitOfWorkResolver
{
    Dictionary<string, Type>? _cache;

    /// <inheritdoc />
    public Type? Resolve(string fullName)
    {
        if (_cache is null || !_cache.ContainsKey(fullName))
        {
            _cache = BuildTypeLookup();
        }
        return _cache.GetValueOrDefault(fullName);
    }

    static Dictionary<string, Type> BuildTypeLookup()
    {
        var cache = new Dictionary<string, Type>();
        var allTypes = GetAllImplementingTypes(typeof(IUnitOfWork));
        foreach (var type in allTypes)
        {
            var fullName = type.FullName;
            if (fullName is null || cache.ContainsKey(fullName))
            {
                continue;
            }
            cache[fullName] = type;
        }
        return cache;
    }

    static Type[] GetAllImplementingTypes(Type contract)
    {
        var entryAssembly = Assembly.GetEntryAssembly()
            ?? throw new InvalidOperationException("Could not find the entry assembly.");
        var dependencyModel = DependencyContext.Load(entryAssembly)
            ?? throw new InvalidOperationException("Could not load the dependency context.");

        var projectReferencedAssemblies = dependencyModel.RuntimeLibraries
            .Where(_ => _.Type.Equals("project"))
            .Select(_ => Assembly.Load(_.Name))
            .ToArray();

        var assemblies = dependencyModel.RuntimeLibraries
            .Where(_ => _.RuntimeAssemblyGroups.Count > 0)
            .Select(_ =>
            {
                try
                {
                    return Assembly.Load(_.Name);
                }
                catch
                {
                    return null!;
                }
            })
            .Where(_ => _ is not null)
            .Distinct()
            .ToList();

        assemblies.AddRange(projectReferencedAssemblies);
        var allTypes = assemblies.SelectMany(_ => _.GetTypes());
        var filteredTypes = allTypes.Where(type =>
        {
            return !type.IsInterface && !type.IsAbstract
            && AllBaseAndImplementingTypes(type).Contains(contract);
        });
        return filteredTypes.Distinct().ToArray();
    }

    static IEnumerable<Type> ThisAndMaybeOpenType(Type type)
    {
        yield return type;
        if (type.GetTypeInfo().IsGenericType && !type.GetTypeInfo().ContainsGenericParameters)
        {
            yield return type.GetGenericTypeDefinition();
        }
    }

    /// <summary>
    /// Returns self and all base and implementing types of a given type, both open and
    /// closed generic (if any).
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to get base and implementing
    /// types for.</param>
    /// <returns>All base and implementing <see cref="Type">types</see>.</returns>
    static IEnumerable<Type> AllBaseAndImplementingTypes(Type type)
    {
        return BaseTypes(type)
            .Concat(type.GetTypeInfo().GetInterfaces())
            .SelectMany(ThisAndMaybeOpenType)
            .Where(t => t != type && t != typeof(object));
    }

    static IEnumerable<Type> BaseTypes(Type type)
    {
        var currentType = type;
        while (currentType != null)
        {
            yield return currentType;
            currentType = currentType.GetTypeInfo().BaseType;
        }
    }
}
