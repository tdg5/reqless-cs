using Reqless.Client.Models;

namespace Reqless.Framework;

/// <summary>
/// Abstraction encapsulating the logic to pop a job for execution.
/// </summary>
public interface IJobReserver
{
    /// <summary>
    /// Tries to reserve a job.
    /// </summary>
    /// <param name="job">The job that was popped to be executed, if any.</param>
    /// <returns>True if a job was reserved, false otherwise.</returns>
    bool TryReserveJob(out Job? job);
}