using Moq;
using Reqless.Extensions.Hosting.Reflection;
using Reqless.Framework;
using Reqless.Tests.Common.TestHelpers;
using Reqless.Tests.Fixtures.UnitOfWork;
using System.Reflection;

namespace Reqless.Extensions.Hosting.Tests.Reflection;

/// <summary>
/// Tests for <see cref="TypeImplementationResolver"/>.
/// </summary>
public class TypeImplementationResolverTest
{
    /// <summary>
    /// <see cref="TypeImplementationResolver.GetBaseTypes"/> should throw when
    /// the given type is null.
    /// </summary>
    [Fact]
    public void GetBaseTypes_ThrowsWhenGivenTypeIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => TypeImplementationResolver.GetBaseTypes(null!),
            "type"
        );
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetBaseTypes"/> should enumerate
    /// the expected sequence of types.
    /// </summary>
    [Fact]
    public void GetBaseTypes_GetsTheExpectedBaseTypes()
    {
        Assert.Equal(
            [
                typeof(ThirdLevelType),
                typeof(SecondLevelType),
                typeof(FirstLevelType),
                typeof(object),
            ],
            TypeImplementationResolver.GetBaseTypes(typeof(ThirdLevelType))
        );
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAssemblies"/> should throw if
    /// the entry assembly cannot be retrieved.
    /// </summary>
    [Fact]
    public void GetAssemblies_ThrowsWhenTheEntryAssemblyIsUnavailable()
    {
        var reflectionGlobalsMock = new Mock<IReflectionGlobals>();
        reflectionGlobalsMock
            .Setup(_ => _.GetEntryAssembly())
            .Returns<Assembly>(null!);

        var subject = MakeSubject(reflectionGlobalsMock.Object);
        var exception = Assert.Throws<InvalidOperationException>(
            () => subject.GetAssemblies()
        );
        Assert.Equal("Could not get the entry assembly.", exception.Message);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAssemblies"/> should throw if
    /// the dependency model can't be loaded.
    /// </summary>
    [Fact]
    public void GetAssemblies_ThrowsWhenTheDependencyModelCannotBeLoaded()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        Assert.NotNull(entryAssembly);
        var reflectionGlobalsMock = new Mock<IReflectionGlobals>();
        reflectionGlobalsMock
            .Setup(_ => _.GetEntryAssembly())
            .Returns(entryAssembly);
        reflectionGlobalsMock
            .Setup(_ => _.LoadDependencyContext(entryAssembly))
            .Returns<AppDomain>(null!);

        var subject = MakeSubject(reflectionGlobalsMock.Object);
        var exception = Assert.Throws<InvalidOperationException>(
            () => subject.GetAssemblies()
        );
        Assert.Equal(
            "Could not load the dependency context for the entry assembly.",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.TryLoadAssemblyByName"/> should return
    /// false and output null when expected exceptions are thrown.
    /// </summary>
    /// <param name="expectedExceptionType">The expected exception type.</param>
    [Theory]
    [InlineData(typeof(BadImageFormatException))]
    [InlineData(typeof(FileNotFoundException))]
    [InlineData(typeof(FileLoadException))]
    [InlineData(typeof(ReflectionTypeLoadException))]
    public void TryLoadAssemblyByName_ReturnsFalseWhenExpectedExceptionsAreThrown(
        Type expectedExceptionType
    )
    {
        Exception expectedException =
            expectedExceptionType == typeof(ReflectionTypeLoadException)
            ? new ReflectionTypeLoadException(null, null)
            : (Exception)Activator.CreateInstance(expectedExceptionType)!;

        var reflectionGlobalsMock = new Mock<IReflectionGlobals>();
        reflectionGlobalsMock
            .Setup(_ => _.LoadAssemblyByName(It.IsAny<string>()))
            .Throws(expectedException);
        var subject = MakeSubject(reflectionGlobalsMock.Object);
        var result = subject.TryLoadAssemblyByName("Ignored", out var assembly);
        Assert.False(result);
        Assert.Null(assembly);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.TryLoadAssemblyByName"/> should return
    /// true and output the expected assembly when all goes according to plan.
    /// </summary>
    [Fact]
    public void TryLoadAssemblyByName_ReturnsTrueAndTheExpectedAssembly()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        Assert.NotNull(entryAssembly);
        var assemblyName = entryAssembly.FullName;
        Assert.NotNull(assemblyName);
        var subject = MakeSubject();
        var result = subject.TryLoadAssemblyByName(
            assemblyName,
            out var assembly
        );
        Assert.True(result);
        Assert.Equal(entryAssembly, assembly);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.TryGetTypes"/> should return false
    /// and output null when getting types causes <see
    /// cref="ReflectionTypeLoadException"/>.
    /// </summary>
    [Fact]
    public void TryGetTypes_ReturnsFalseWhenGettingTypesThrowsReflectionTypeLoadException()
    {
        var assemblyMock = new Mock<Assembly>();
        assemblyMock
            .Setup(_ => _.GetTypes())
            .Throws(new ReflectionTypeLoadException(null, null));
        var assembly = assemblyMock.Object;
        var result = TypeImplementationResolver.TryGetTypes(assembly, out var types);
        Assert.False(result);
        Assert.Null(types);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.TryGetTypes"/> should return true and
    /// expected types when all goes according to plan.
    /// </summary>
    [Fact]
    public void TryGetTypes_ReturnsTrueAndExpectedTypes()
    {
        var assembly = Assembly.GetEntryAssembly();
        Assert.NotNull(assembly);
        var result = TypeImplementationResolver.TryGetTypes(assembly, out var types);
        Assert.True(result);
        Assert.Equal(assembly.GetTypes(), types);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// throw when the given type is null.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_ThrowsWhenGivenTypeIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => MakeSubject().GetAllImplementingTypes(null!),
            "type"
        );
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// ignore assemblies for which types fail to load due to <see
    /// cref="ReflectionTypeLoadException"/>.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_IgnoresAssembliesForWhichTypesFailToLoad()
    {
        var assemblyMock = new Mock<Assembly>();
        assemblyMock
            .Setup(_ => _.GetTypes())
            .Throws(new ReflectionTypeLoadException(null, null));
        var subject = new StaticAssembliesTypeImplementationResolver([
            assemblyMock.Object
        ]);
        Assert.Empty(subject.GetAllImplementingTypes(typeof(object)));
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// be able to resolve a type that directly implements <see
    /// cref="IUnitOfWork"/>.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_ResolvesTypeThatDirectlyImplementsIUnitOfWork()
    {
        var subject = MakeSubject();
        var expectedType = typeof(UnitOfWorkImplementer);
        var actualTypes = subject.GetAllImplementingTypes(typeof(IUnitOfWork));
        Assert.Contains(expectedType, actualTypes);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// be able to resolve an <see cref="IUnitOfWork"/> type based on a generic
    /// type that implements <see cref="IUnitOfWork"/>.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_ResolvesClosedGenericUnitOfWork()
    {
        var subject = MakeSubject();
        var expectedType = typeof(ClosedGenericNoopUnitOfWork);
        var actualTypes = subject.GetAllImplementingTypes(typeof(IUnitOfWork));
        Assert.Contains(expectedType, actualTypes);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// be able to resolve an <see cref="IUnitOfWork"/> type that does not implement
    /// <see cref="IUnitOfWork"/> directly, but inherits from a class that does.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_ResolvesSubclassOfUnitOfWorkType()
    {
        var subject = MakeSubject();
        var expectedType = typeof(UnitOfWorkImplementerSubclass);
        var actualTypes = subject.GetAllImplementingTypes(typeof(IUnitOfWork));
        Assert.Contains(expectedType, actualTypes);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// be able to resolve an <see cref="IUnitOfWork"/> type that does not implement
    /// <see cref="IUnitOfWork"/> directly, but implements another interface that does.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_ResolvesTypeThatImplementsInterfaceImplementingIUnitOfWork()
    {
        var subject = MakeSubject();
        var expectedType = typeof(NoopUnitOfWorkWithExtras);
        var actualTypes = subject.GetAllImplementingTypes(typeof(IUnitOfWork));
        Assert.Contains(expectedType, actualTypes);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// be able to resolve an <see cref="IUnitOfWork"/> type that does not
    /// implement <see cref="IUnitOfWork"/> directly, but implements another
    /// interface that includes a default implementation.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_ResolvesTypeThatImplementsInterfaceWithDefaultIUnitOfWorkImplementation()
    {
        var subject = MakeSubject();
        var expectedType = typeof(NoopUnitOfWorkImplementer);
        var actualTypes = subject.GetAllImplementingTypes(typeof(IUnitOfWork));
        Assert.Contains(expectedType, actualTypes);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// resolve an internal class that implements the <see cref="IUnitOfWork"/>
    /// interface.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_ResolvesInternalUnitOfWork()
    {
        var subject = MakeSubject();
        var expectedTypeName = "InternalUnitOfWorkImplementer";
        var actualTypes = subject.GetAllImplementingTypes(typeof(IUnitOfWork));
        Assert.Contains(actualTypes, _ => _.Name == expectedTypeName);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// resolve a private class that implements the <see cref="IUnitOfWork"/>
    /// interface.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_ResolvesPrivateUnitOfWork()
    {
        var subject = MakeSubject();
        var expectedTypeName = "PrivateUnitOfWorkImplementer";
        var actualTypes = subject.GetAllImplementingTypes(typeof(IUnitOfWork));
        Assert.Contains(actualTypes, _ => _.Name == expectedTypeName);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// resolve a protected class that implements the <see cref="IUnitOfWork"/>
    /// interface.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_ResolvesProtectedUnitOfWork()
    {
        var subject = MakeSubject();
        var expectedTypeName = "ProtectedUnitOfWorkImplementer";
        var actualTypes = subject.GetAllImplementingTypes(typeof(IUnitOfWork));
        Assert.Contains(actualTypes, _ => _.Name == expectedTypeName);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// be able to resolve an <see cref="IUnitOfWork"/> type that does not implement
    /// <see cref="IUnitOfWork"/> directly, but completes the implementation of
    /// an abstract class that does.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_ResolvesConcreteSubclassOfAbstractUnitOfWorkType()
    {
        var subject = MakeSubject();
        var expectedType = typeof(ConcreteNoopUnitOfWork);
        var actualTypes = subject.GetAllImplementingTypes(typeof(IUnitOfWork));
        Assert.Contains(expectedType, actualTypes);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// be able to resolve an <see cref="IUnitOfWork"/> type that is created
    /// dynamically as part of a dynamic assembly.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_ResolvesDynamicUnitOfWorkType()
    {
        Type dynamicUnitOfWork = NoopUnitOfWorkTypeFactory.Create(
            "DynamicUnitOfWorkAssembly",
            "DynamicUnitOfWorkNamespace",
            $"{nameof(TypeImplementationResolverTest)}DynamicUnitOfWork"
        );

        var subject = MakeSubject();
        var actualTypes = subject.GetAllImplementingTypes(typeof(IUnitOfWork));
        Assert.Contains(dynamicUnitOfWork, actualTypes);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// not resolve an abstract class that includes the <see cref="IUnitOfWork"/>
    /// interface, but does not actually implement it.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_DoesNotResolveAbstractUnitOfWork()
    {
        var subject = MakeSubject();
        var expectedType = typeof(AbstractUnitOfWork);
        var actualTypes = subject.GetAllImplementingTypes(typeof(IUnitOfWork));
        Assert.DoesNotContain(expectedType, actualTypes);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// not resolve an open generic class that implements the <see
    /// cref="IUnitOfWork"/>.  interface.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_DoesNotResolveOpenGenericUnitOfWork()
    {
        var subject = MakeSubject();
        var expectedType = typeof(GenericNoopUnitOfWork<string>).GetGenericTypeDefinition();
        var actualTypes = subject.GetAllImplementingTypes(typeof(IUnitOfWork));
        Assert.DoesNotContain(expectedType, actualTypes);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// not resolve an interface that includes the <see cref="IUnitOfWork"/>
    /// interface and includes a default implementation.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_DoesNotResolveInterfaceThatFullyImplementsIUnitOfWork()
    {
        var subject = MakeSubject();
        var expectedType = typeof(INoopUnitOfWork);
        var actualTypes = subject.GetAllImplementingTypes(typeof(IUnitOfWork));
        Assert.DoesNotContain(expectedType, actualTypes);
    }

    /// <summary>
    /// <see cref="TypeImplementationResolver.GetAllImplementingTypes"/> should
    /// not resolve an interface that includes the <see cref="IUnitOfWork"/>
    /// interface.
    /// </summary>
    [Fact]
    public void GetAllImplementingTypes_DoesNotResolveInterfaceWithUnitOfWork()
    {
        var subject = MakeSubject();
        var expectedType = typeof(IUnitOFWorkWithExtras);
        var actualTypes = subject.GetAllImplementingTypes(typeof(IUnitOfWork));
        Assert.DoesNotContain(expectedType, actualTypes);
    }

    private static TypeImplementationResolver MakeSubject(
        IReflectionGlobals? reflectionGlobals = null
    )
    {
        var _reflectionGlobals = reflectionGlobals ?? new DefaultReflectionGlobals();
        return new TypeImplementationResolver(_reflectionGlobals);
    }

    private class FirstLevelType { }

    private class SecondLevelType : FirstLevelType { }

    private class ThirdLevelType : SecondLevelType { }

    private class StaticAssembliesTypeImplementationResolver
        : TypeImplementationResolver
    {
        private List<Assembly> _assemblies;

        public StaticAssembliesTypeImplementationResolver(
            List<Assembly> assemblies
        )
        {
            ArgumentNullException.ThrowIfNull(assemblies, nameof(assemblies));
            _assemblies = assemblies;
        }

        internal protected override IEnumerable<Assembly> GetAssemblies()
        {
            return _assemblies;
        }
    }
}
