using Moq;
using Reqless.Extensions.Hosting.Reflection;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Framework;
using Reqless.Tests.Common.TestHelpers;
using Reqless.Tests.Fixtures.UnitOfWork;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Tests for <see cref="DefaultUnitOfWorkResolver"/>.
/// </summary>
public class DefaultUnitOfWorkResolverTest
{
    /// <summary>
    /// <see cref="DefaultUnitOfWorkResolver.Resolve"/> should throw when given
    /// type name argument is null.
    /// </summary>
    [Fact]
    public void Resolve_ThrowsWhenGivenTypeIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new DefaultUnitOfWorkResolver().Resolve(null!),
            "typeName");
    }

    /// <summary>
    /// <see cref="DefaultUnitOfWorkResolver.Resolve"/> should be able to
    /// resolve an <see cref="IUnitOfWork"/> type.
    /// </summary>
    [Fact]
    public void Resolve_ResolvesType()
    {
        var subject = new DefaultUnitOfWorkResolver();
        var type = typeof(UnitOfWorkImplementer);
        var actualType = subject.Resolve(type.FullName!);
        Assert.Equal(type, actualType);
    }

    /// <summary>
    /// <see cref="DefaultUnitOfWorkResolver.Resolve"/> should be able to
    /// resolve an <see cref="IUnitOfWork"/> type that was created after the
    /// type cache was created.
    /// </summary>
    [Fact]
    public void Resolve_ResolvesTypeCreatedAfterTheTypeCacheWasCreated()
    {
        var subject = new DefaultUnitOfWorkResolver();

        // Ensure the cache has been populated.
        var initialType = typeof(ClosedGenericNoopUnitOfWork);
        var actualInitialType = subject.Resolve(initialType.FullName!);
        Assert.Equal(initialType, actualInitialType);

        string assemblyName = "DynamicUnitOfWorkAssembly";
        string namespaceName = "DynamicUnitOfWorkNamespace";
        string typeName = $"{nameof(DefaultUnitOfWorkResolverTest)}DynamicUnitOfWork";
        string expectedDynamicUnitOfWorkTypeName = $"{namespaceName}.{typeName}";
        Assert.Throws<TypeLoadException>(
            () => subject.Resolve(expectedDynamicUnitOfWorkTypeName));

        Type dynamicUnitOfWork = NoopUnitOfWorkTypeFactory.Create(
            assemblyName, namespaceName, typeName);
        Assert.Equal(expectedDynamicUnitOfWorkTypeName, dynamicUnitOfWork.FullName);

        var typeAgain = Type.GetType($"{expectedDynamicUnitOfWorkTypeName}, {assemblyName}");
        Assert.Equal(dynamicUnitOfWork, subject.Resolve(dynamicUnitOfWork.FullName!));
    }

    /// <summary>
    /// <see cref="DefaultUnitOfWorkResolver.Resolve"/> should throw when two
    /// types with the same full name are encountered.
    /// </summary>
    [Fact]
    public void Resolve_ThrowsIfTwoTypesWithTheSameFullNameAreEncountered()
    {
        StaticTypesTypeImplementationResolver typeImplementationResolver = new([
            typeof(NoopUnitOfWorkImplementer),
            typeof(NoopUnitOfWorkImplementer)
        ]);
        DefaultUnitOfWorkResolver subject = new(typeImplementationResolver);
        var exception = Assert.Throws<InvalidOperationException>(
            () => subject.Resolve("Ignored"));
        Assert.Equal(
            $"Multiple types implementing {typeof(IUnitOfWork).FullName} with the"
            + $" full name '{typeof(NoopUnitOfWorkImplementer).FullName}' were found.",
            exception.Message);
    }

    /// <summary>
    /// <see cref="DefaultUnitOfWorkResolver.Resolve"/> should ignore types that
    /// do not have a full name.
    /// </summary>
    [Fact]
    public void Resolve_IgnoresTypesWithoutAFullName()
    {
        var mockType = new Mock<Type>();
        mockType.Setup(_ => _.FullName).Returns((string?)null);
        StaticTypesTypeImplementationResolver typeImplementationResolver = new([
            mockType.Object,
        ]);
        var typeName = "NoSuchType";
        DefaultUnitOfWorkResolver subject = new(typeImplementationResolver);
        var exception = Assert.Throws<TypeLoadException>(
            () => subject.Resolve(typeName));
        Assert.Equal(
            $"No type implementing {typeof(IUnitOfWork).FullName} with the"
            + $" full name '{typeName}' was found.",
            exception.Message);
    }

    /// <summary>
    /// <see cref="DefaultUnitOfWorkResolver.Resolve"/> should throw if no type
    /// is found with the given full name.
    /// </summary>
    [Fact]
    public void Resolve_ThrowsWhenNoTypeWithGivenFullNameIsFound()
    {
        var typeName = "NoSuchType";
        DefaultUnitOfWorkResolver subject = new();
        var exception = Assert.Throws<TypeLoadException>(
            () => subject.Resolve(typeName));
        Assert.Equal(
            $"No type implementing {typeof(IUnitOfWork).FullName} with the"
            + $" full name '{typeName}' was found.",
            exception.Message);
    }

    private class StaticTypesTypeImplementationResolver : ITypeImplementationResolver
    {
        private readonly Type[] _types;

        public StaticTypesTypeImplementationResolver(Type[] types)
        {
            _types = types;
        }

        public Type[] GetAllImplementingTypes(Type type)
        {
            return _types;
        }
    }
}
