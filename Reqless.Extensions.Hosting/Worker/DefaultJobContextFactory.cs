using Reqless.Client.Models;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Default concrete implementation of <see cref="IJobContextFactory"/>.
/// </summary>
public class DefaultJobContextFactory : IJobContextFactory
{
    /// <inheritdoc/>
    public IJobContext Create(IServiceProvider serviceProvider, Job job)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        ArgumentNullException.ThrowIfNull(job, nameof(job));

        return new DefaultJobContext(job);
    }
}
