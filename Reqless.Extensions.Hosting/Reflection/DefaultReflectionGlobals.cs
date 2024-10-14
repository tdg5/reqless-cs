using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace Reqless.Extensions.Hosting.Reflection;

/// <summary>
/// Default implementation of <see cref="IReflectionGlobals"/> wrapping the
/// actual reflection globals of interest.
/// </summary>
public class DefaultReflectionGlobals : IReflectionGlobals
{
    /// <inheritdoc/>
    public AppDomain CurrentAppDomain => AppDomain.CurrentDomain;

    /// <inheritdoc/>
    public Assembly? GetEntryAssembly()
    {
        return Assembly.GetEntryAssembly();
    }

    /// <inheritdoc/>
    public Assembly LoadAssemblyByName(string assemblyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyName, nameof(assemblyName));
        return Assembly.Load(assemblyName);
    }

    /// <inheritdoc/>
    public DependencyContext? LoadDependencyContext(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));
        return DependencyContext.Load(assembly);
    }
}
