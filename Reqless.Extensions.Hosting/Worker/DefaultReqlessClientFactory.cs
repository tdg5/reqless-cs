using Reqless.Client;
using Reqless.Framework;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Default implementation of <see cref="IReqlessClientFactory"/> that uses an
/// instance of <see cref="WorkerSettings"/> for client configuration.
/// </summary>
public class DefaultReqlessClientFactory : IReqlessClientFactory
{
    private readonly IWorkerSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultReqlessClientFactory"/>
    /// class.
    /// </summary>
    /// <param name="settings">The <see cref="WorkerSettings"/> instance
    /// that should be used to configure the clients.</param>
    public DefaultReqlessClientFactory(IWorkerSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings, nameof(settings));

        _settings = settings;
    }

    /// <inheritdoc />
    public IReqlessClient Create()
    {
        return new ReqlessClient(_settings.ConnectionString);
    }
}
