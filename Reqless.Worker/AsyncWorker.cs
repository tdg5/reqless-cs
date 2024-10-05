using Reqless.Client.Models;
using Reqless.Extensions.Hosting.Worker;

namespace Reqless.Worker;

/// <summary>
/// <see cref="IWorker"/> that performs jobs serially in an async context.
/// </summary>
public class AsyncWorker : IWorker
{
    /// <summary>
    /// An <see cref="IJobExecutor"/> instance to use wrapping the execution of
    /// jobs.
    /// </summary>
    protected readonly IJobExecutor _jobExecutor;

    /// <summary>
    /// An <see cref="IJobReserver"/> instance to use for reserving jobs.
    /// </summary>
    protected readonly IJobReserver _jobReserver;

    /// <inheritdoc/>
    public string Name { get; }

    /// <summary>
    /// Create an instance of <see cref="AsyncWorker"/>.
    /// </summary>
    /// <param name="jobExecutor">An <see cref="IJobExecutor"/> instance to use
    /// for wrapping the execution of jobs.</param>
    /// <param name="jobReserver">An <see cref="IJobReserver"/> instance to use
    /// for reserving jobs.</param>
    /// <param name="name">The name the worker should use when communicating
    /// with Reqless.</param>
    public AsyncWorker(IJobExecutor jobExecutor, IJobReserver jobReserver, string name)
    {
        _jobExecutor = jobExecutor;
        _jobReserver = jobReserver;
        Name = name;
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Job? job = await _jobReserver.TryReserveJobAsync(Name);
                if (job is not null)
                {
                    await _jobExecutor.ExecuteAsync(
                        job,
                        cancellationToken
                    );
                }
            }
            finally
            {
            }
            await Task.Delay(1_000, cancellationToken);
        }
    }
}
