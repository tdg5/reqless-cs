using Reqless.Models.JobEvents;
using Reqless.Serialization;
using System.Text.Json.Serialization;

namespace Reqless.Models;

/// <summary>
/// Data class representing a job, as defined by qless-core.
/// </summary>
[JsonConverter(typeof(JobJsonConverter))]
public class Job
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
    /// The JIDs of jobs that must complete before the job is eligible to run.
    /// </summary>
    public string[] Dependencies { get; }

    /// <summary>
    /// The JIDs of jobs that depend on the completion of the job before they
    /// can run.
    /// </summary>
    public string[] Dependents { get; }

    /// <summary>
    /// The time at which the lease on the job will expire causing the job to
    /// be considered lost and become eligible for scheduling again.
    /// </summary>
    public long? Expires { get; }

    /// <summary>
    /// Information about the last time the job failed, if any.
    /// </summary>
    public JobFailure? Failure { get; }

    /// <summary>
    /// A list of events that have happened to the job over the course of its
    /// lifetime.
    /// </summary>
    public JobEvent[] History { get; }

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
    /// The number of retry attempts remaining for the job before all retries
    /// are exhausted.
    /// </summary>
    public int Remaining { get; }

    /// <summary>
    /// The total number of times the job will been retried before the job is
    /// declared failed.
    /// </summary>
    public int Retries { get; }

    /// <summary>
    /// The JID, if any, of the job that spawned the job.
    /// </summary>
    public string? SpawnedFromJid { get; }

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
    /// Flag indicating whether or not the job is being tracked.
    /// </summary>
    public bool Tracked { get; }

    /// <summary>
    /// The name of the worker that is currently working on the job, if any.
    /// </summary>
    public string? WorkerName { get; }

    /// <summary>
    /// Fully construct an instance of a job, tyically from a JSON payload given
    /// by the server.
    /// </summary>
    /// <param name="className">The name of the class that will be used to
    /// perform the job.</param>
    /// <param name="data">The stringifed JSON data relevant to the job that
    /// will be passed to the job class at execution time.</param>
    /// <param name="dependencies">The JIDs of jobs that must complete before
    /// the job is eligible to run.</param>
    /// <param name="dependents">The JIDs of jobs that depend on the completion
    /// of the job before they can run.</param>
    /// <param name="expires">The time at which the lease on the job will
    /// expire causing the job to be considered lost and become eligible for
    /// scheduling again.</param>
    /// <param name="failure">Information about the last time the job failed,
    /// if any.</param>
    /// <param name="history">A list of events that have happened to the job
    /// over the course of its lifetime.</param>
    /// <param name="jid">The unique identifier, or job ID, of the job.</param>
    /// <param name="priority">The priority of the job, which determines the
    /// order in which jobs are popped off the queue, with a lower value
    /// representing a more urgent priority.</param>
    /// <param name="queueName">The name of the queue that the job currently
    /// belongs to.</param>
    /// <param name="remaining">The number of retry attempts remaining for the
    /// job before all retries are exhausted.</param>
    /// <param name="retries">The total number of times the job will been
    /// retried before the job is declared failed.</param>
    /// <param name="spawnedFromJid">The JID, if any, of the job that spawned
    /// the job</param>
    /// <param name="state">The current state of the job.</param>
    /// <param name="tags">A list of tags that are applied to the job for
    /// tracking purposes.</param>
    /// <param name="throttles">A list of throttles that are applied to the job
    /// to manage various concurrency limits and prevent the job from being
    /// scheduled when capacity is not available.</param>
    /// <param name="tracked">Flag indicatin whether or not the job is being
    /// tracked.</param>
    /// <param name="workerName">The name of the worker that is currently
    /// working on the job, if any.</param>
    public Job(
        string className,
        string data,
        string[] dependencies,
        string[] dependents,
        long? expires,
        JobFailure? failure,
        JobEvent[] history,
        string jid,
        int priority,
        string? queueName,
        int remaining,
        int retries,
        string? spawnedFromJid,
        string state,
        string[] tags,
        string[] throttles,
        bool tracked,
        string? workerName
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(className, nameof(className));
        ArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));
        ValidationHelper.ThrowIfAnyNullOrWhitespace(dependencies, nameof(dependencies));
        ValidationHelper.ThrowIfAnyNullOrWhitespace(dependents, nameof(dependents));
        ValidationHelper.ThrowIfAnyNull(history, nameof(history));
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));
        // Queue name can be null when a job is completed and no longer in a queue.
        ValidationHelper.ThrowIfNotNullAndEmptyOrWhitespace(queueName, nameof(queueName));
        ValidationHelper.ThrowIfNotNullAndEmptyOrWhitespace(spawnedFromJid, nameof(spawnedFromJid));
        ArgumentException.ThrowIfNullOrWhiteSpace(state, nameof(state));
        ValidationHelper.ThrowIfAnyNullOrWhitespace(tags, nameof(tags));
        ValidationHelper.ThrowIfAnyNullOrWhitespace(throttles, nameof(throttles));
        ValidationHelper.ThrowIfNotNullAndEmptyOrWhitespace(workerName, nameof(workerName));
        if (expires <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expires),
                expires,
                "expires must be a positive whole number."
            );
        }
        if (priority < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(priority),
                priority,
                "priority must be a non-negative whole number."
            );
        }
        if (remaining < -1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(remaining),
                remaining,
                "remaining must be a whole number greater than or equal to -1."
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
        if (remaining > retries)
        {
            throw new ArgumentOutOfRangeException(
                nameof(remaining),
                remaining,
                $"remaining must be less than or equal to retries ({retries})."
            );
        }

        ClassName = className;
        Data = data;
        Dependencies = dependencies;
        Dependents = dependents;
        Expires = expires;
        Failure = failure;
        History = history;
        Jid = jid;
        Priority = priority;
        QueueName = queueName;
        Remaining = remaining;
        Retries = retries;
        SpawnedFromJid = spawnedFromJid;
        State = state;
        Tags = tags;
        Throttles = throttles;
        Tracked = tracked;
        WorkerName = workerName;
    }
}