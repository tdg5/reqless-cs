using Reqless.Client;

namespace Reqless.Framework;

/// <summary>
/// Represents a factory that creates instances of <see cref="IReqlessClient"/>.
/// </summary>
public class DelegatingReqlessClientFactory : IReqlessClientFactory
{
    readonly Func<IReqlessClient> _clientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegatingReqlessClientFactory"/> class.
    /// </summary>
    /// <param name="clientFactory">A function returning an <see
    /// cref="IReqlessClient"/> instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref
    /// name="clientFactory"/> is null.</exception>
    public DelegatingReqlessClientFactory(Func<IReqlessClient> clientFactory)
    {
        ArgumentNullException.ThrowIfNull(clientFactory, nameof(clientFactory));

        _clientFactory = clientFactory;
    }

    /// <inheritdoc />
    public IReqlessClient Create() => _clientFactory();
}
