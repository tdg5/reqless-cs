using Reqless.Client;
using Reqless.Client.Models;
using Reqless.Extensions.Hosting.Worker;

namespace Reqless.Worker;

/// <summary>
/// Default implementation of <see cref="IJobReserver"/>.
/// </summary>
public class DefaultJobReserver : IJobReserver
{
    private readonly IReqlessClient _reqlessClient;

    private readonly IQueueNameProvider _queueNameProvider;

    /// <summary>
    /// Create an instance of <see cref="DefaultJobReserver"/>.
    /// </summary>
    /// <param name="queueNameProvider">The <see cref="IQueueNameProvider"/>
    /// that will provide queue names for the job reserver to use.</param>
    /// <param name="reqlessClient">The <see cref="IReqlessClient"/> to use for
    /// reserving jobs.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DefaultJobReserver(
        IQueueNameProvider queueNameProvider,
        IReqlessClient reqlessClient
    )
    {
        ArgumentNullException.ThrowIfNull(
            queueNameProvider,
            nameof(queueNameProvider)
        );
        ArgumentNullException.ThrowIfNull(reqlessClient, nameof(reqlessClient));

        _queueNameProvider = queueNameProvider;
        _reqlessClient = reqlessClient;
    }

    /// <inheritdoc />
    public async Task<Job?> TryReserveJobAsync(
        string workerName,
        CancellationToken? cancellationToken = null
    )
    {
        var _cancellationToken = cancellationToken ?? CancellationToken.None;
        var queueNames = await _queueNameProvider.GetQueueNamesAsync();
        foreach (var queueName in queueNames)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var job = await _reqlessClient.PopJobAsync(queueName, workerName);
            if (job is not null)
            {
                return job;
            }
        }
        return null;
    }
}
