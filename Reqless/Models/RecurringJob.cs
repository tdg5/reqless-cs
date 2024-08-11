using Reqless.Serialization;
using Reqless.Validation;
using System.Text.Json.Serialization;

namespace Reqless.Models;

/// <summary>
/// Data class representing the definition of a recurring job.
/// </summary>
[JsonConverter(typeof(RecurringJobJsonConverter))]
public class RecurringJob : BaseJob
{
    /// <summary>
    /// A counter tracking the number of times the job has been previously
    /// executed, used to generate unique job IDs.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// The interval in seconds with which the job should recur.
    /// </summary>
    public int IntervalSeconds { get; }

    /// <summary>
    /// The maximum number of backlogged jobs that can be scheduled at once to
    /// make up for skipped jobs that occurred as a result of a flood of jobs or
    /// due to resource contention.
    /// </summary>
    public int MaximumBacklog { get; }

    /// <summary>
    /// Construct an instance of <see cref="RecurringJob"/>.
    /// </summary>
    /// <param name="className">The name of the class that will be used to
    /// perform the job.</param>
    /// <param name="count">The number of times the job has been run
    /// previously.</param>
    /// <param name="data">The stringifed JSON data relevant to the job that
    /// will be passed to the job class at execution time.</param>
    /// <param name="intervalSeconds">The interval in seconds with which the job
    /// should recur.</param>
    /// <param name="jid">The unique identifier, or job ID, of the job.</param>
    /// <param name="maximumBacklog">The maximum number of backlogged jobs that
    /// can be scheduled at once to make up for skipped jobs that occurred as a
    /// result of a flood of jobs or due to resource contention.</param>
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
    public RecurringJob(
        string className,
        int count,
        string data,
        int intervalSeconds,
        string jid,
        int maximumBacklog,
        int priority,
        string? queueName,
        int retries,
        string state,
        string[] tags,
        string[] throttles
    ) : base(className, data, jid, priority, queueName, retries, state, tags, throttles)
    {
        ArgumentValidation.ThrowIfNegative(count, nameof(count));
        ArgumentValidation.ThrowIfNotPositive(intervalSeconds, nameof(intervalSeconds));
        ArgumentValidation.ThrowIfNegative(maximumBacklog, nameof(maximumBacklog));

        Count = count;
        IntervalSeconds = intervalSeconds;
        MaximumBacklog = maximumBacklog;
    }
}