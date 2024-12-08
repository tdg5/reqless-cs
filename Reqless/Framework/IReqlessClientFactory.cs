using Reqless.Client;

namespace Reqless.Framework;

/// <summary>
/// Represents a factory that creates instances of <see cref="IReqlessClient"/>.
/// </summary>
public interface IReqlessClientFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IReqlessClient"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="IReqlessClient"/>.</returns>
    IReqlessClient Create();
}
