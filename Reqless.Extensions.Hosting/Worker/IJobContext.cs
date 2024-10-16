using Reqless.Client.Models;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Represents the context of a job that is actively being processed.
/// </summary>
public interface IJobContext
{
    /// <summary>
    /// The job that is actively being processed.
    /// </summary>
    Job Job { get; }
}
