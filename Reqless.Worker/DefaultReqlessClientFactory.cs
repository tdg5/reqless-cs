using Reqless.Client;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Framework;

namespace Reqless.Worker;

/// <summary>
/// Default implementation of <see cref="IReqlessClientFactory"/> that uses
/// an instance of <see cref="WorkerSettings"/> for client configuration.
/// </summary>
public class DefaultReqlessClientFactory : IReqlessClientFactory
{
    private IWorkerSettings _settings;

    /// <summary>
    /// Create an instance of <see cref="DefaultReqlessClientFactory"/>.
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
