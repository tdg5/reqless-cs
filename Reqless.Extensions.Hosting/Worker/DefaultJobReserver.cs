using Reqless.Client;
using Reqless.Client.Models;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Default implementation of <see cref="IJobReserver"/>.
/// </summary>
public class DefaultJobReserver : IJobReserver
{
    private readonly IQueueNameProvider _queueNameProvider;

    private readonly IReqlessClient _reqlessClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultJobReserver"/> class.
    /// </summary>
    /// <param name="queueNameProvider">The <see cref="IQueueNameProvider"/>
    /// that will provide queue names for the job reserver to use.</param>
    /// <param name="reqlessClient">The <see cref="IReqlessClient"/> to use for
    /// reserving jobs.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="queueNameProvider"/>
    /// or <paramref name="reqlessClient"/> is <see langword="null"/>.</exception>
    public DefaultJobReserver(
        IQueueNameProvider queueNameProvider, IReqlessClient reqlessClient)
    {
        ArgumentNullException.ThrowIfNull(
            queueNameProvider, nameof(queueNameProvider));
        ArgumentNullException.ThrowIfNull(reqlessClient, nameof(reqlessClient));

        _queueNameProvider = queueNameProvider;
        _reqlessClient = reqlessClient;
    }

    /// <inheritdoc />
    public async Task<Job?> TryReserveJobAsync(
        string workerName,
        CancellationToken? cancellationToken = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workerName, nameof(workerName));

        var actualCancellationToken = cancellationToken ?? CancellationToken.None;
        var queueNames = await _queueNameProvider.GetQueueNamesAsync();
        foreach (var queueName in queueNames)
        {
            if (actualCancellationToken.IsCancellationRequested)
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
