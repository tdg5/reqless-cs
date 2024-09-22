using Reqless.Client;
using Reqless.Client.Models;

namespace Reqless.Framework;

/// <summary>
/// Default implementation of <see cref="IJobReserver"/>.
/// </summary>
public class DefaultJobReserver : IJobReserver
{
    IReqlessClientAccessor ReqlessClientAccessor { get; }

    /// <summary>
    /// Create an instance of <see cref="DefaultJobReserver"/>.
    /// </summary>
    /// <param name="reqlessClientAccessor"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public DefaultJobReserver(
        IReqlessClientAccessor reqlessClientAccessor
    )
    {
        ArgumentNullException.ThrowIfNull(reqlessClientAccessor, nameof(reqlessClientAccessor));
        ReqlessClientAccessor = reqlessClientAccessor;
    }

    /// <inheritdoc />
    public Task<Job?> TryReserveJobAsync()
    {
        IReqlessClient reqlessClient = ReqlessClientAccessor.Value ??
            throw new InvalidOperationException(
                "The ReqlessClientAccessor returned null."
            );

        Job? job = null;
        return Task.FromResult(job);
    }
}
