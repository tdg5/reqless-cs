using Reqless.Client.Models;

namespace Reqless.Framework;

/// <summary>
/// Represents a factory that creates instances of <see cref="IJobContext"/>.
/// </summary>
public interface IJobContextFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IJobContext"/>.
    /// </summary>
    /// <param name="job">The job that is actively being processed.</param>
    /// <returns></returns>
    IJobContext Create(Job job);
}