using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace Reqless.Extensions.Hosting.Reflection;

/// <summary>
/// Interface for wrapping global constants related to reflection to aid in testing.
/// </summary>
public interface IReflectionGlobals
{
    /// <summary>
    /// Gets the current application domain for the current Thread. By default,
    /// a wrapper around <see cref="AppDomain.CurrentDomain"/>.
    /// </summary>
    /// <returns>The current application domain</returns>
    public AppDomain CurrentAppDomain { get; }

    /// <summary>
    /// Get the entry assembly for the current application. By default, a wrapper
    /// around <see cref="Assembly.GetEntryAssembly"/>.
    /// </summary>
    /// <returns>The assembly that is the process executable in the default
    /// application domain, or the first executable that was executed by <see
    /// cref="AppDomain.ExecuteAssembly(string)"/>. Can return null when called
    /// from unmanaged code.</returns>
    public Assembly? GetEntryAssembly();

    /// <summary>
    /// Load an assembly by name. By default, a wrapper around <see
    /// cref="Assembly.Load(string)"/>.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to load.</param>
    /// <returns>The loaded assembly.</returns>
    public Assembly LoadAssemblyByName(string assemblyName);

    /// <summary>
    /// Load the <see cref="DependencyContext"/> for the given <paramref
    /// name="assembly"/>.  By default, a wrapper around <see
    /// cref="DependencyContext.Load(Assembly)"/>.
    /// </summary>
    /// <param name="assembly">The assembly to load the dependency context
    /// for.</param>
    /// <returns>The dependency context for the specified assembly or null when
    /// the dependency context is not available.</returns>
    public DependencyContext? LoadDependencyContext(Assembly assembly);
}
