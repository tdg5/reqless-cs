using Reqless.Client.Models;

namespace Reqless.Worker;

/// <summary>
/// Abstraction for adding logic around the execution of a job.
/// </summary>
public interface IJobExecutor
{
    /// <summary>
    /// Execute a job.
    /// </summary>
    /// <param name="job">The job to execute.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>
    /// indicating whether or not work on the job should be discontinued prior
    /// to completion.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous
    /// operation.</returns>
    Task ExecuteAsync(Job job, CancellationToken cancellationToken);
}
