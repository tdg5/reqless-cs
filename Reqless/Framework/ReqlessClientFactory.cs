using Reqless.Client;

namespace Reqless.Framework;

/// <summary>
/// Represents a factory that creates instances of <see cref="IReqlessClient"/>.
/// </summary>
public class ReqlessClientFactory : IReqlessClientFactory
{
    Func<IReqlessClient> _clientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReqlessClientFactory"/> class.
    /// </summary>
    /// <param name="clientFactory">A function returning an <see
    /// cref="IReqlessClient"/> instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref
    /// name="clientFactory"/> is null.</exception>
    public ReqlessClientFactory(Func<IReqlessClient> clientFactory)
    {
        _clientFactory = clientFactory
            ?? throw new ArgumentNullException(nameof(clientFactory));
    }

    /// <inheritdoc />
    public IReqlessClient Create() => _clientFactory();
}
