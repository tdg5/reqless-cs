namespace Reqless.Framework;

/// <summary>
/// Contract that must be implemented by classes that perform work as part of a
/// Reqless job.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Do the work.
    /// </summary>
    /// <returns></returns>
    Task PerformAsync();
}