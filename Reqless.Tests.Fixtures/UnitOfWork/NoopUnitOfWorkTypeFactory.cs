using Reqless.Framework;
using System.Reflection;
using System.Reflection.Emit;

namespace Reqless.Tests.Fixtures.UnitOfWork;

/// <summary>
/// A type factory that creates new types that implements <see
/// cref="IUnitOfWork"/> and do nothing.
/// </summary>
public static class NoopUnitOfWorkTypeFactory
{
    /// <summary>
    /// Creates a new type that implements <see cref="IUnitOfWork"/> and does
    /// nothing.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to create for the
    /// new type.</param>
    /// <param name="namespaceName">The name of the namespace to use for the new
    /// type.</param>
    /// <param name="typeName">The name of the type to create.</param>
    /// <param name="moduleName">The name of the module to create for the new
    /// type.</param>
    /// <returns>A new type that implements <see cref="IUnitOfWork"/> and does
    /// nothing.</returns>
    public static Type Create(
        string assemblyName,
        string namespaceName,
        string typeName,
        string? moduleName = null
    )
    {
        string _moduleName = moduleName ?? assemblyName;
        AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName(assemblyName),
            AssemblyBuilderAccess.Run
        );

        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(_moduleName);

        string _typeName = $"{namespaceName}.{typeName}";
        Type iUnitOfWorkType = typeof(IUnitOfWork);
        TypeBuilder typeBuilder = moduleBuilder.DefineType(
            attr: TypeAttributes.Public | TypeAttributes.Class,
            interfaces: [iUnitOfWorkType],
            name: _typeName,
            parent: null
        );

        Type cancellationTokenType = typeof(CancellationToken);
        string completedTaskName = nameof(Task.CompletedTask);
        string performAsyncName = nameof(IUnitOfWork.PerformAsync);
        MethodBuilder performAsyncBuilder = typeBuilder.DefineMethod(
            attributes: MethodAttributes.Public | MethodAttributes.Virtual,
            callingConvention: CallingConventions.HasThis,
            name: performAsyncName,
            parameterTypes: [cancellationTokenType],
            returnType: typeof(Task)
        );

        var completedTaskPropertyInfo = typeof(Task).GetProperty(
            completedTaskName,
            BindingFlags.Public | BindingFlags.Static
        ) ?? throw new InvalidOperationException(
            $"Could not find the {completedTaskName} property."
        );
        var completedTaskPropertyGetMethod = completedTaskPropertyInfo.GetGetMethod()
            ?? throw new InvalidOperationException(
                $"Could not find the get method of the {completedTaskName} property."
            );

        var performAsyncBody = performAsyncBuilder.GetILGenerator();
        performAsyncBody.Emit(OpCodes.Call, completedTaskPropertyGetMethod);
        performAsyncBody.Emit(OpCodes.Ret);

        var unitOfWorkPerformAsyncMethod =
            iUnitOfWorkType.GetMethod(performAsyncName, [cancellationTokenType])
            ?? throw new InvalidOperationException(
                $"Could not find the {performAsyncName} method."
            );
        typeBuilder.DefineMethodOverride(
            performAsyncBuilder,
            unitOfWorkPerformAsyncMethod
        );

        Type type = typeBuilder.CreateType();
        SanityCheckType(type);
        return type;
    }

    private static void SanityCheckType(Type type)
    {
        string typeFullName = type.FullName
            ?? throw new InvalidOperationException("The type does not have a full name.");
        Type iUnitOfWorkType = typeof(IUnitOfWork);
        // Make sure the runtime acknowledges the type as IUnitOfWork.
        if (!iUnitOfWorkType.IsAssignableFrom(type))
        {
            throw new InvalidOperationException(
                $"{typeFullName} does not implement {iUnitOfWorkType.FullName}."
            );
        }

        // Make sure it's possible to create an instance.
        var instance = Activator.CreateInstance(type)
            ?? throw new InvalidOperationException(
                $"Could not create an instance of {typeFullName}."
            );

        // Make sure the PerformAsync method works when invoked on the instance
        // as the exact type.
        string performAsyncName = nameof(IUnitOfWork.PerformAsync);
        string completedTaskName = nameof(Task.CompletedTask);
        var typePerformAsync = type.GetMethod(
            performAsyncName,
            [typeof(CancellationToken)]
        ) ?? throw new InvalidOperationException(
            $"Could not find the {typeFullName}.{performAsyncName} method."
        );
        var directResult = typePerformAsync.Invoke(instance, [CancellationToken.None]);
        if (directResult != Task.CompletedTask)
        {
            throw new InvalidOperationException(
                $"The {typeFullName}.{performAsyncName} method did not return"
                    + $" {completedTaskName}."
            );
        }

        // Make sure the PerformAsync method works when invoked on the instance
        // through the IUnitOfWork interface.
        var unitOfWorkResult = ((IUnitOfWork)instance).PerformAsync(CancellationToken.None);
        if (unitOfWorkResult != Task.CompletedTask)
        {
            throw new InvalidOperationException(
                $"The {nameof(IUnitOfWork)}.{performAsyncName} method did not"
                    + $" return {completedTaskName}."
            );
        }
    }
}
