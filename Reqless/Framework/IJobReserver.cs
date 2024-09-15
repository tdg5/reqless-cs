using System.Diagnostics.CodeAnalysis;
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
    /// <returns>A task containing a job if one could be reserved, otherwise
    /// null.</returns>
    Task<Job?> TryReserveJobAsync();
}