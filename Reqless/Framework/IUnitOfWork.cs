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
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>
    /// indicating whether or not work on the job should be discontinued prior
    /// to completion.</param>
    /// <returns>Task representing the completion of the work.</returns>
    Task PerformAsync(CancellationToken cancellationToken);
}
