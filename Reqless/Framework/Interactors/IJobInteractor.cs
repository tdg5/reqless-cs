namespace Reqless.Framework.Interactors;

/// <summary>
/// Interface for types that represent jobs that can be referenced and
/// interacted with.
/// </summary>
public interface IJobInteractor
{
    /// <summary>
    /// The name of the <see cref="IUnitOfWork"/> class that will be used to
    /// perform the job.
    /// </summary>
    string ClassName { get; }

    /// <summary>
    /// The unique identifier, or job ID, of the job.
    /// </summary>
    string Jid { get; }

    /// <summary>
    /// The priority of the job, which, ignoring other factors like throttles,
    /// determines the order in which jobs are popped off the queue. A lower
    /// value represents a less urgent priority and a higher value represents a
    /// more urgent priority.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// The name of the queue that the job currently belongs to.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="Reqless.Client.Models.BaseJob"/>, queue name should
    /// never be null since this interface should only be used to model jobs in
    /// an incomplete state.
    /// </remarks>
    string QueueName { get; }

    /// <summary>
    /// The total number of times the job will been retried before the job is
    /// declared failed.
    /// </summary>
    int Retries { get; }

    /// <summary>
    /// The current state of the job.
    /// </summary>
    string State { get; }

    /// <summary>
    /// A list of tags that are applied to the job for tracking purposes.
    /// </summary>
    string[] Tags { get; }

    /// <summary>
    /// A list of throttles that are applied to the job to manage various
    /// concurrency limits and prevent the job from being scheduled when
    /// capacity is not available.
    /// </summary>
    string[] Throttles { get; }
}
