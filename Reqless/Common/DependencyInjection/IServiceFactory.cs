namespace Reqless.Common.DependencyInjection;

/// <summary>
/// A factory for creating services of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the service that should be
/// created.</typeparam>
public interface IServiceFactory<T>
{
    /// <summary>
    /// Build a service of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>An instance of type <typeparamref name="T"/>.</returns>
    public T Build();
}