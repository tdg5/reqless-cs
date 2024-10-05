using Reqless.Client.Models;

namespace Reqless.Worker;

/// <summary>
/// Default concrete implementation of the <see cref="IJobContext"/> interface.
/// </summary>
public class DefaultJobContext : IJobContext
{
    /// <inheritdoc />
    public Job Job { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultJobContext"/> class.
    /// </summary>
    /// <param name="job">The job that is actively being processed.</param>
    public DefaultJobContext(Job job)
    {
        Job = job;
    }
}
