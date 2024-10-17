using Microsoft.Extensions.Logging;
using Reqless.Client.Models;

namespace Reqless.Extensions.Hosting.Worker;

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

    /// <summary>
    /// An <see cref="ILogger{TCategoryName}"/> instance to use for logging.
    /// </summary>
    protected readonly ILogger<AsyncWorker> _logger;

    /// <inheritdoc/>
    public string Name { get; }

    /// <summary>
    /// Create an instance of <see cref="AsyncWorker"/>.
    /// </summary>
    /// <param name="jobExecutor">An <see cref="IJobExecutor"/> instance to use
    /// for wrapping the execution of jobs.</param>
    /// <param name="jobReserver">An <see cref="IJobReserver"/> instance to use
    /// for reserving jobs.</param>
    /// <param name="logger">An <see cref="ILogger{TCategoryName}"/> instance to
    /// use for logging.</param>
    /// <param name="name">The name the worker should use when communicating
    /// with Reqless.</param>
    public AsyncWorker(
        IJobExecutor jobExecutor,
        IJobReserver jobReserver,
        ILogger<AsyncWorker> logger,
        string name
    )
    {
        ArgumentNullException.ThrowIfNull(jobExecutor, nameof(jobExecutor));
        ArgumentNullException.ThrowIfNull(jobReserver, nameof(jobReserver));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        _jobExecutor = jobExecutor;
        _jobReserver = jobReserver;
        _logger = logger;
        Name = name;
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (
                    await _jobReserver.TryReserveJobAsync(Name, cancellationToken)
                    is Job job
                )
                {
                    await _jobExecutor.ExecuteAsync(
                        job,
                        cancellationToken
                    );
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while processing a job.");
            }
        }
    }
}
