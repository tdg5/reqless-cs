using Reqless.Client.Models;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Represents the context of a job that is actively being processed.
/// </summary>
public interface IJobContext
{
    /// <summary>
    /// Gets the job that is actively being processed.
    /// </summary>
    Job Job { get; }
}
