using Reqless.Client;
using Reqless.Client.Models;

namespace Reqless.Framework;

/// <summary>
/// Default implementation of <see cref="IJobReserver"/>.
/// </summary>
public class DefaultJobReserver : IJobReserver
{
    private IReqlessClient _reqlessClient { get; }

    /// <summary>
    /// Create an instance of <see cref="DefaultJobReserver"/>.
    /// </summary>
    /// <param name="reqlessClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public DefaultJobReserver(
        IReqlessClient reqlessClient
    )
    {
        ArgumentNullException.ThrowIfNull(reqlessClient, nameof(reqlessClient));
        _reqlessClient = reqlessClient;
    }

    /// <inheritdoc />
    public Task<Job?> TryReserveJobAsync()
    {
        Job? job = null;
        return Task.FromResult(job);
    }
}
