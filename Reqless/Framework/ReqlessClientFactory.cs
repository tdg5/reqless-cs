using Reqless.Client;

namespace Reqless.Framework;

/// <summary>
/// Represents a factory that creates instances of <see cref="IClient"/>.
/// </summary>
public class ReqlessClientFactory : IReqlessClientFactory
{
    Func<IClient> _clientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReqlessClientFactory"/> class.
    /// </summary>
    /// <param name="clientFactory">A function returning an <see
    /// cref="IClient"/> instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref
    /// name="clientFactory"/> is null.</exception>
    public ReqlessClientFactory(Func<IClient> clientFactory)
    {
        _clientFactory = clientFactory
            ?? throw new ArgumentNullException(nameof(clientFactory));
    }

    /// <inheritdoc />
    public IClient Create() => _clientFactory();
}