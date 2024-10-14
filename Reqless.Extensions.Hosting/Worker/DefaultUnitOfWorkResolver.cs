using Reqless.Extensions.Hosting.Reflection;
using Reqless.Framework;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Concrete implmentation of <see cref="IUnitOfWorkResolver"/> that uses
/// reflection to resolve types.
/// </summary>
public class DefaultUnitOfWorkResolver : IUnitOfWorkResolver
{
    Dictionary<string, Type>? _cache;

    private readonly ITypeImplementationResolver _typeImplementationResolver;

    /// <summary>
    /// Create a new instance of <see cref="DefaultUnitOfWorkResolver"/>.
    /// </summary>
    /// <param name="typeImplementationResolver">An optional <see
    /// cref="ITypeImplementationResolver"/> instance that should be used to
    /// resolve types.</param>
    public DefaultUnitOfWorkResolver(
        ITypeImplementationResolver? typeImplementationResolver = null
    )
    {
        _typeImplementationResolver =
            typeImplementationResolver ?? new TypeImplementationResolver();
    }

    /// <inheritdoc />
    public Type? Resolve(string typeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName, nameof(typeName));

        if (_cache is null || !_cache.ContainsKey(typeName))
        {
            _cache = BuildTypeLookup(typeof(IUnitOfWork));
        }
        // Since a failed lookup invalidates the cache, throw an exception to
        // discourage further lookups with the same type name.
        var result = _cache.GetValueOrDefault(typeName)
            ?? throw new TypeLoadException(
                $"No type implementing {typeof(IUnitOfWork)} with the full name"
                  + $" '{typeName}' was found."
            );
        return result;
    }

    Dictionary<string, Type> BuildTypeLookup(Type targetType)
    {
        var cache = new Dictionary<string, Type>();
        var types = _typeImplementationResolver.GetAllImplementingTypes(targetType);
        foreach (var type in types)
        {
            var fullName = type.FullName;
            if (fullName is null)
            {
                continue;
            }
            else if (cache.ContainsKey(fullName))
            {
                throw new InvalidOperationException(
                    $"Multiple types implementing {targetType} with the"
                      + $" full name '{fullName}' were found."
                );
            }
            cache[fullName] = type;
        }
        return cache;
    }
}
