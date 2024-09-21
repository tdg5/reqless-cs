using Reqless.Client;
using Reqless.Client.Models;

namespace Reqless.Framework;

/// <summary>
/// Default implementation of <see cref="IJobReserver"/>.
/// </summary>
public class DefaultJobReserver : IJobReserver
{
    IReqlessClient ReqlessClient { get; }

    /// <summary>
    /// Create an instance of <see cref="DefaultJobReserver"/>.
    /// </summary>
    /// <param name="reqlessClientAccessor"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public DefaultJobReserver(
        IReqlessClientAccessor reqlessClientAccessor
    )
    {
        ReqlessClient = reqlessClientAccessor.Value ??
            throw new ArgumentNullException(nameof(reqlessClientAccessor));

    }

    /// <inheritdoc />
    public Task<Job?> TryReserveJobAsync()
    {
        Job? job = null;
        return Task.FromResult(job);
    }
}
