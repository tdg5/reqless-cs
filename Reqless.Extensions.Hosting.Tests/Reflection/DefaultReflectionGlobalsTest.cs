using Microsoft.Extensions.DependencyModel;
using Reqless.Extensions.Hosting.Reflection;
using Reqless.Tests.Common.TestHelpers;
using System.Reflection;

namespace Reqless.Extensions.Hosting.Tests.Reflection;

/// <summary>
/// Tests for <see cref="DefaultReflectionGlobals"/>.
/// </summary>
public class DefaultReflectionGlobalsTest
{
    /// <summary>
    /// Tests that <see cref="DefaultReflectionGlobals.CurrentAppDomain"/>
    /// returns the current app domain.
    /// </summary>
    [Fact]
    public void CurrentAppDomain_ShouldReturnCurrentAppDomain()
    {
        var subject = new DefaultReflectionGlobals();
        Assert.Equal(AppDomain.CurrentDomain, subject.CurrentAppDomain);
    }

    /// <summary>
    /// Tests that <see cref="DefaultReflectionGlobals.GetEntryAssembly"/>
    /// returns the entry assembly.
    /// </summary>
    [Fact]
    public void GetEntryAssembly_ShouldReturnEntryAssembly()
    {
        var subject = new DefaultReflectionGlobals();
        Assert.Equal(Assembly.GetEntryAssembly(), subject.GetEntryAssembly());
    }

    /// <summary>
    /// Tests that <see cref="DefaultReflectionGlobals.LoadAssemblyByName"/>
    /// throws when the assembly name is null.
    /// </summary>
    [Fact]
    public void LoadAssemblyByName_ThrowsWhenAssemblyNameIsNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidValue) =>
                new DefaultReflectionGlobals().LoadAssemblyByName(invalidValue!),
            "assemblyName"
        );
    }

    /// <summary>
    /// Tests that <see cref="DefaultReflectionGlobals.LoadAssemblyByName"/>
    /// loads the assembly by name.
    /// </summary>
    [Fact]
    public void LoadAssemblyByName_ShouldLoadAssemblyByName()
    {
        var subject = new DefaultReflectionGlobals();
        var entryAssembly = Assembly.GetEntryAssembly();
        Assert.NotNull(entryAssembly);
        var assemblyName = entryAssembly.FullName;
        Assert.NotNull(assemblyName);
        var assembly = subject.LoadAssemblyByName(assemblyName);
        Assert.NotNull(assembly);
        Assert.Equal(entryAssembly, assembly);
    }

    /// <summary>
    /// Tests that <see cref="DefaultReflectionGlobals.LoadDependencyContext"/>
    /// throws when the assembly is null.
    /// </summary>
    [Fact]
    public void LoadDependencyContext_ThrowsWhenAssemblyIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new DefaultReflectionGlobals().LoadDependencyContext(null!),
            "assembly"
        );
    }

    /// <summary>
    /// Tests that <see cref="DefaultReflectionGlobals.LoadDependencyContext"/>
    /// loads the dependency context for the assembly.
    /// </summary>
    [Fact]
    public void LoadDependencyContext_ShouldLoadDependencyContext()
    {
        var subject = new DefaultReflectionGlobals();
        var entryAssembly = Assembly.GetEntryAssembly();
        Assert.NotNull(entryAssembly);
        var dependencyContext = subject.LoadDependencyContext(entryAssembly);
        Assert.NotNull(dependencyContext);
        var expectedDependencyContext = DependencyContext.Load(entryAssembly);
        Assert.Equivalent(expectedDependencyContext, dependencyContext);
    }
}
