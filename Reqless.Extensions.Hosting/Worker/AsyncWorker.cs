using Microsoft.Extensions.Logging;
using Reqless.Client.Models;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// <see cref="IWorker"/> that performs jobs serially in an async context.
/// </summary>
public class AsyncWorker : IWorker
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncWorker"/> class.
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
        string name)
    {
        ArgumentNullException.ThrowIfNull(jobExecutor, nameof(jobExecutor));
        ArgumentNullException.ThrowIfNull(jobReserver, nameof(jobReserver));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        JobExecutor = jobExecutor;
        JobReserver = jobReserver;
        Logger = logger;
        Name = name;
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <summary>
    /// Gets the <see cref="IJobExecutor"/> instance to use to wrap the
    /// execution of jobs.
    /// </summary>
    protected IJobExecutor JobExecutor { get; }

    /// <summary>
    /// Gets the <see cref="IJobReserver"/> instance to use for reserving jobs.
    /// </summary>
    protected IJobReserver JobReserver { get; }

    /// <summary>
    /// Gets the <see cref="ILogger{TCategoryName}"/> instance to use for logging.
    /// </summary>
    protected ILogger<AsyncWorker> Logger { get; }

    /// <inheritdoc/>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var jobMaybe = await JobReserver.TryReserveJobAsync(
                    Name, cancellationToken);
                if (jobMaybe is Job job)
                {
                    await JobExecutor.ExecuteAsync(job, cancellationToken);
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "An error occurred while processing a job.");
            }
        }
    }
}
