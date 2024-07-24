namespace Reqless.Models;

/// <summary>
/// Data class representing the properties shared by all typed of jobs, as
/// defined by reqless-core.
/// </summary>
public class BaseJob
{
    /// <summary>
    /// The name of the class that will be used to perform the job.
    /// </summary>
    public string ClassName { get; }

    /// <summary>
    /// The stringifed JSON data relevant to the job that will be passed to the
    /// job class at execution time.
    /// </summary>
    public string Data { get; }

    /// <summary>
    /// The unique identifier, or job ID, of the job.
    /// </summary>
    public string Jid { get; }

    /// <summary>
    /// The priority of the job, which, ignoring other factors like throttles,
    /// determines the order in which jobs are popped off the queue. A lower
    /// value represents a less urgent priority and a higher value represents a
    /// more urgent priority.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// The name of the queue that the job currently belongs to.
    /// </summary>
    public string? QueueName { get; }

    /// <summary>
    /// The total number of times the job will been retried before the job is
    /// declared failed.
    /// </summary>
    public int Retries { get; }

    /// <summary>
    /// The current state of the job.
    /// </summary>
    public string State { get; }

    /// <summary>
    /// A list of tags that are applied to the job for tracking purposes.
    /// </summary>
    public string[] Tags { get; }

    /// <summary>
    /// A list of throttles that are applied to the job to manage various
    /// concurrency limits and prevent the job from being scheduled when
    /// capacity is not available.
    /// </summary>
    public string[] Throttles { get; }

    /// <summary>
    /// Fully construct an instance of a job, tyically from a JSON payload given
    /// by the server.
    /// </summary>
    /// <param name="className">The name of the class that will be used to
    /// perform the job.</param>
    /// <param name="data">The stringifed JSON data relevant to the job that
    /// will be passed to the job class at execution time.</param>
    /// <param name="jid">The unique identifier, or job ID, of the job.</param>
    /// <param name="priority">The priority of the job, which determines the
    /// order in which jobs are popped off the queue, with a lower value
    /// representing a more urgent priority.</param>
    /// <param name="queueName">The name of the queue that the job currently
    /// belongs to.</param>
    /// <param name="retries">The total number of times the job will been
    /// retried before the job is declared failed.</param>
    /// <param name="state">The current state of the job.</param>
    /// <param name="tags">A list of tags that are applied to the job for
    /// tracking purposes.</param>
    /// <param name="throttles">A list of throttles that are applied to the job
    /// to manage various concurrency limits and prevent the job from being
    /// scheduled when capacity is not available.</param>
    public BaseJob(
        string className,
        string data,
        string jid,
        int priority,
        string? queueName,
        int retries,
        string state,
        string[] tags,
        string[] throttles
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(className, nameof(className));
        ArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));
        // Queue name can be null when a job is completed and no longer in a queue.
        ValidationHelper.ThrowIfNotNullAndEmptyOrWhitespace(queueName, nameof(queueName));
        ArgumentException.ThrowIfNullOrWhiteSpace(state, nameof(state));
        ValidationHelper.ThrowIfAnyNullOrWhitespace(tags, nameof(tags));
        ValidationHelper.ThrowIfAnyNullOrWhitespace(throttles, nameof(throttles));
        if (priority < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(priority),
                priority,
                "priority must be a non-negative whole number."
            );
        }
        if (retries < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(retries),
                retries,
                "retries must be a non-negative whole number."
            );
        }

        ClassName = className;
        Data = data;
        Jid = jid;
        Priority = priority;
        QueueName = queueName;
        Retries = retries;
        State = state;
        Tags = tags;
        Throttles = throttles;
    }
}