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
    IReqlessClient Create();
}
