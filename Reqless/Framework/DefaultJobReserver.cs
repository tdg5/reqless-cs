using Reqless.Client;
using Reqless.Client.Models;
using Reqless.Framework.QueueIdentifierResolvers;

namespace Reqless.Framework;

/// <summary>
/// Default implementation of <see cref="IJobReserver"/>.
/// </summary>
public class DefaultJobReserver : IJobReserver
{
    private readonly IReqlessClient _reqlessClient;

    private readonly IQueueIdentifierResolver _queueIdentifierResolver;

    /// <summary>
    /// Create an instance of <see cref="DefaultJobReserver"/>.
    /// </summary>
    /// <param name="queueIdentifierResolver">The <see cref="IQueueIdentifierResolver"/>
    /// to use for resolving queue identifiers.</param>
    /// <param name="reqlessClient">The <see cref="IReqlessClient"/> to use for
    /// reserving jobs.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DefaultJobReserver(
        IQueueIdentifierResolver queueIdentifierResolver,
        IReqlessClient reqlessClient
    )
    {
        ArgumentNullException.ThrowIfNull(
            queueIdentifierResolver,
            nameof(queueIdentifierResolver)
        );
        ArgumentNullException.ThrowIfNull(reqlessClient, nameof(reqlessClient));

        _queueIdentifierResolver = queueIdentifierResolver;
        _reqlessClient = reqlessClient;
    }

    /// <inheritdoc />
    public Task<Job?> TryReserveJobAsync()
    {
        // Should this method take queue identifiers as an argument?
        Job? job = null;
        return Task.FromResult(job);
    }
}
