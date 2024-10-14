using Reqless.Client.Models;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Abstraction encapsulating the logic to pop a job for execution.
/// </summary>
public interface IJobReserver
{
    /// <summary>
    /// Tries to reserve a job.
    /// </summary>
    /// <param name="workerName">The name of the worker that is trying to
    /// reserve a job.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to
    /// cancel the operation.</param>
    /// <returns>A task containing a job if one could be reserved, otherwise
    /// null.</returns>
    Task<Job?> TryReserveJobAsync(
        string workerName,
        CancellationToken? cancellationToken = null
    );
}
