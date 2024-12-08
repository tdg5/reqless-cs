using Reqless.Client.Models;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Represents a factory that creates instances of <see cref="IJobContext"/>.
/// </summary>
public interface IJobContextFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IJobContext"/>.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/>
    /// instance that should be used to create the <see cref="IJobContext"/>
    /// instance.</param>
    /// <param name="job">The <see cref="Job"/> that is actively being
    /// processed.</param>
    /// <returns>A new instance of <see cref="IJobContext"/>.</returns>
    IJobContext Create(IServiceProvider serviceProvider, Job job);
}
