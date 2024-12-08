using Reqless.Client.Models;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Default concrete implementation of the <see cref="IJobContext"/> interface.
/// </summary>
public class DefaultJobContext : IJobContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultJobContext"/> class.
    /// </summary>
    /// <param name="job">The job that is actively being processed.</param>
    public DefaultJobContext(Job job)
    {
        ArgumentNullException.ThrowIfNull(job, nameof(job));

        Job = job;
    }

    /// <inheritdoc />
    public Job Job { get; }
}
