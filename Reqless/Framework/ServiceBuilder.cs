using Microsoft.Extensions.DependencyInjection;

namespace Reqless.Framework;

/// <summary>
/// Helper class for building services from factories taking an <see
/// cref="IServiceProvider"/> as an argument.
/// </summary>
public static class ServiceBuilder
{
    /// <summary>
    /// Create a service of type <typeparamref name="T"/> from a factory that
    /// takes an <see cref="IServiceProvider"/> as an argument.
    /// </summary>
    /// <typeparam name="F">The type of the factory that should be used to
    /// create the service. The factory must implement <see
    /// cref="IServiceFactory{T}"/>.</typeparam>
    /// <typeparam name="T">The type of the service that should be
    /// created.</typeparam>
    /// <param name="factory">The <see cref="IServiceFactory{T}"/> instance
    /// that should be used to create the service.</param>
    /// <returns>An instance of type <typeparamref name="T"/> or <see
    /// langword="null"/> if no such service should be constructed.</returns>
    public static Func<IServiceProvider, T?> FromFactory<F, T>(F factory)
    where F : notnull, IServiceFactory<T>
    {
        return serviceProvider =>
            ActivatorUtilities.CreateInstance<F>(serviceProvider).Build();
    }
}
