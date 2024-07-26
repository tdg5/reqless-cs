using System.Text.Json;
using Reqless.Models;

namespace Reqless.Client;

/// <summary>
/// Interface for a client that can interact with a Redish-backed Reqless
/// server.
/// </summary>
public interface IClient
{
    /// <summary>
    /// Update the given job to add a dependency on the job identified by the
    /// given jid.
    /// </summary>
    /// <param name="jid">The ID of the job that should depend on the given
    /// jid.</param>
    /// <param name="dependsOnJid">The ID of the job that the given job should
    /// depend on.</param>
    Task<bool> AddDependencyToJobAsync(string jid, string dependsOnJid);

    /// <summary>
    /// Add an event to the history of the job with the given jid.
    /// </summary>
    /// <param name="jid">The ID of the job to add an event to.</param>
    /// <param name="what">The what or name for the event.</param>
    /// <param name="data">Optional data for the event.</param>
    Task<bool> AddEventToJobHistoryAsync(
        string jid,
        string what,
        string? data = null
    );

    /// <summary>
    /// Update the given job to add the given tag.
    /// </summary>
    /// <param name="jid">The ID of the job that the tag should be added to.</param>
    /// <param name="tag">The tag that should be added to the job.</param>
    Task<List<string>> AddTagToJobAsync(string jid, string tag);

    /// <summary>
    /// Update the given job to add the given tags.
    /// </summary>
    /// <param name="jid">The ID of the job that should have tags added.</param>
    /// <param name="tags">The tags that should be added to the job.</param>
    Task<List<string>> AddTagsToJobAsync(string jid, params string[] tags);

    /// <summary>
    /// Cancel a job.
    /// </summary>
    /// <param name="jid">The ID of the job that should be cancelled.</param>
    Task<bool> CancelJobAsync(string jid);

    /// <summary>
    /// Cancel one or more jobs.
    /// </summary>
    /// <param name="jids">The IDs of the jobs that should be cancelled.</param>
    Task<bool> CancelJobsAsync(params string[] jids);

    /// <summary>
    /// Complete the job with the given jid.
    /// </summary>
    /// <param name="jid">The ID of the job that should be completed.</param>
    /// <param name="workerName">The name of the worker that completed the
    /// job.</param>
    /// <param name="queueName">The name of the queue the job was in.</param>
    /// <param name="data">The data that was processed or updated data for
    /// future processing.</param>
    /// <returns>True if the job was successfully completed, false if the job
    /// does not exist, is owned by another worker, or is not currently
    /// running.</returns>
    Task<bool> CompleteJobAsync(
        string jid,
        string workerName,
        string queueName,
        string data
    );

    /// <summary>
    /// Complete the job with the given jid and requeue it to the given queue.
    /// </summary>
    /// <param name="jid">The ID of the job that should be completed.</param>
    /// <param name="workerName">The name of the worker that completed the
    /// job.</param>
    /// <param name="queueName">The name of the queue the job was in.</param>
    /// <param name="data">The data that was processed or updated data for
    /// future processing.</param>
    /// <param name="nextQueueName">The name of the queue to put the job in
    /// next.</param>
    /// <param name="delay">The delay in seconds before the job is processed
    /// again.</param>
    /// <param name="depends">The job IDs that the should depend on after it is
    /// requeued.</param>
    /// <returns>True if the job was successfully completed, false if the job
    /// does not exist, is owned by another worker, or is not currently
    /// running.</returns>
    Task<bool> CompleteAndRequeueJobAsync(
        string jid,
        string workerName,
        string queueName,
        string data,
        string nextQueueName,
        int delay = 0,
        string[]? depends = null
    );

    /// <summary>
    /// Fails a job.
    /// </summary>
    /// <param name="jid">The job ID.</param>
    /// <param name="workerName">The name of the worker that failed the job.</param>
    /// <param name="group">The group or kind of the failure.</param>
    /// <param name="message">The message for the failure.</param>
    /// <param name="data">Optional, updated data for the job.</param>
    Task<bool> FailJobAsync(
        string jid,
        string workerName,
        string group,
        string message,
        string? data = null
    );

    /// <summary>
    /// Returns a dictionary where each key is a known failure group and each
    /// value is the count of jobs that have failed with that group.
    /// </summary>
    Task<Dictionary<string, int>> FailureGroupsCountsAsync();

    /// <summary>
    /// Gets the IDs of jobs that have completed successfully.
    /// </summary>
    /// <param name="limit">The maximum number of job IDs to retrieve.</param>
    /// <param name="offset">The number of job IDs to skip before returning
    /// results.</param>
    /// <returns>Zero or more job IDs of completed jobs.</returns>
    Task<List<string>> GetCompletedJobsAsync(
        int limit = 25,
        int offset = 0
    );

    /// <summary>
    /// Gets the IDs of jobs that have failed with the given failure group.
    /// </summary>
    /// <param name="group">The name of the failure group to retrieve job
    /// failures for.</param>
    /// <param name="limit">The maximum number of job IDs to retrieve.</param>
    /// <param name="offset">The number of job IDs to skip before returning
    /// results.</param>
    Task<JidsResult> GetFailedJobsByGroupAsync(
        string group,
        int limit = 25,
        int offset = 0
    );

    /// <summary>
    /// Forget the queue with the given name.
    /// </summary>
    /// <param name="queueName">The name of the queue that should be removed
    /// from the set of known queues.</param>
    Task ForgetQueueAsync(string queueName);

    /// <summary>
    /// Forget the queues with the given names.
    /// </summary>
    /// <param name="queueNames">The names of the queue that should be removed
    /// from the set of known queues.</param>
    Task ForgetQueuesAsync(params string[] queueNames);

    /// <summary>
    /// Get all Reqless configuration values.
    /// </summary>
    /// <returns>Zero or more job IDs of completed jobs.</returns>
    Task<Dictionary<string, JsonElement>> GetAllConfigsAsync();

    /// <summary>
    /// Get the counts of the number of jobs in various states for all queues.
    /// </summary>
    Task<List<QueueCounts>> GetAllQueueCountsAsync();

    /// <summary>
    /// Gets a job by its job ID.
    /// </summary>
    /// <param name="jid">The ID of the job that should be retreived.</param>
    /// <returns>The job, if it exists; otherwise, null.</returns>
    Task<Job?> GetJobAsync(string jid);

    /// <summary>
    /// Get one or more jobs by their job IDs.
    /// </summary>
    /// <param name="jids">The IDs of the jobs that should be retreived.</param>
    /// <returns>An array of the retrieved jobs.</returns>
    Task<List<Job>> GetJobsAsync(params string[] jids);

    /// <summary>
    /// Gets the IDs of jobs in the given queue in the given state.
    /// </summary>
    /// <param name="state">The state of the jobs to retrieve IDs for.</param>
    /// <param name="queueName">The name of the queue to retrieve job IDs
    /// from.</param>
    /// <param name="limit">The maximum number of job IDs to retrieve.</param>
    /// <param name="offset">The number of job IDs to skip before returning
    /// results.</param>
    /// <returns>Zero or more job IDs matching the given arguments.</returns>
    Task<List<string>> GetJobsByStateAsync(
        string state,
        string queueName,
        int limit = 25,
        int offset = 0
    );

    /// <summary>
    /// Gets the IDs of jobs that have the given tag.
    /// </summary>
    /// <param name="tag">The name of the tag to retrieve jobs for.</param>
    /// <param name="limit">The maximum number of job IDs to retrieve.</param>
    /// <param name="offset">The number of job IDs to skip before returning
    /// results.</param>
    Task<JidsResult> GetJobsByTagAsync(
        string tag,
        int limit = 25,
        int offset = 0
    );

    /// <summary>
    /// Get the counts of the number of jobs in various states in the queue with
    /// the given name.
    /// </summary>
    /// <param name="queueName">The name of the queue to get the counts of.</param>
    Task<QueueCounts> GetQueueCountsAsync(string queueName);

    /// <summary>
    /// Get the count of jobs in the given queue.
    /// </summary>
    /// <param name="queueName">The name of the queue to get the length of.</param>
    Task<int> GetQueueLengthAsync(string queueName);

    /// <summary>
    /// Get statistics on the durations that jobs spent in different states for
    /// the queue with the given name for the given date;
    /// </summary>
    /// <param name="queueName">The name of the queue to retrieve stats
    /// for.</param>
    /// <param name="date">The date to retrieve stats for. Defaults to now.
    /// Reqless will round the value to midnight of the given day, so exact
    /// precision is not required.</param>
    Task<QueueStats> GetQueueStatsAsync(string queueName, DateTimeOffset? date);

    /// <summary>
    /// Get the throttle for the queue with the given name.
    /// </summary>
    /// <param name="queueName">The name of the queue to retrieve the throttle
    /// for.</param>
    Task<Throttle> GetQueueThrottleAsync(string queueName);

    /// <summary>
    /// Gets a recurring job by its job ID.
    /// </summary>
    /// <param name="jid">The ID of the recurring job that should be
    /// retreived.</param>
    /// <returns>The job, if it exists; otherwise, null.</returns>
    Task<RecurringJob?> GetRecurringJobAsync(string jid);

    /// <summary>
    /// Gets the details of all currently traced jobs.
    /// </summary>
    Task<TrackedJobsResult> GetTrackedJobsAsync();

    /// <summary>
    /// Renew the lease on the job with the given jid. Throws if the job does
    /// not exist, is not running, or is not locked by the given worker.
    /// </summary>
    /// <param name="jid">The ID of the job to renew the lease on.</param>
    /// <param name="workerName">The name of the worker that is requesting the
    /// lease be renewed.</param>
    /// <param name="data">Optional updated data for the job.</param>
    /// <returns>The new expiration time of the job.</returns>
    Task<long> HeartbeatJobAsync(
        string jid,
        string workerName,
        string? data = null
    );

    /// <summary>
    /// Pause the queue with the given name.
    /// </summary>
    /// <param name="queueName">The name of the queue that should be
    /// paused.</param>
    Task PauseQueueAsync(string queueName);

    /// <summary>
    /// Peek jobs from the given queue starting from the given offset and taking
    /// at most limit jobs.
    /// </summary>
    /// <param name="queueName">The name of the queue to peek jobs from.</param>
    /// <param name="limit">The maximum number of job IDs to retrieve.</param>
    /// <param name="offset">The number of job IDs to skip before returning
    /// results.</param>
    /// <returns>Zero or more job IDs of completed jobs.</returns>
    Task<List<Job>> PeekJobsAsync(string queueName, int limit, int offset);

    /// <summary>
    /// Attempt to pop a job from a queue.
    /// </summary>
    /// <param name="queueName">The name of the queue to take work from.</param>
    /// <param name="workerName">The name of the worker that is requesting
    /// work.</param>
    /// <returns>The job, if one was popped; otherwise, null.</returns>
    Task<Job?> PopJobAsync(string queueName, string workerName);

    /// <summary>
    /// Attempt to pop multiple jobs from a queue.
    /// </summary>
    /// <param name="queueName">The name of the queue to take work from.</param>
    /// <param name="workerName">The name of the worker that is requesting
    /// work.</param>
    /// <param name="count">The maximum number of jobs to pop.</param>
    /// <returns>The jobs that were popped, if any.</returns>
    Task<List<Job>> PopJobsAsync(
        string queueName,
        string workerName,
        int count
    );

    /// <summary>
    /// Puts a job on a queue.
    /// </summary>
    /// <param name="workerName">The name of the worker or client that's putting
    /// the job.</param>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="className">The name of the class.</param>
    /// <param name="data">The data to be processed.</param>
    /// <param name="delay">The delay in seconds before the job is
    /// processed. Defaults to no delay (0).</param>
    /// <param name="jid">The ID of the job. When not provided, an ID will be
    /// generated.</param>
    /// <param name="priority">The priority of the job. Defaults to 0,
    /// representing a low priority. Ignoring other factors like throttles, jobs
    /// with a higher priority value are popped before jobs with a lower
    /// priority.</param>
    /// <param name="retries">The number of retries.</param>
    /// <param name="dependencies">The job IDs that this job depends on.</param>
    /// <param name="tags">The tags for the job.</param>
    /// <param name="throttles">The throttles for the job.</param>
    /// <returns>The job ID.</returns>
    Task<string> PutJobAsync(
        string workerName,
        string queueName,
        string className,
        string data,
        int delay = 0,
        string? jid = null,
        int priority = 0,
        int retries = 5,
        string[]? dependencies = null,
        string[]? tags = null,
        string[]? throttles = null
    );

    /// <summary>
    /// Puts a job on a queue that runs repeatedly at the given interval.
    /// </summary>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="className">The name of the class.</param>
    /// <param name="data">The data to be processed.</param>
    /// <param name="intervalSeconds">The interval in seconds with which the job
    /// should recur.</param>
    /// <param name="initialDelaySeconds">The delay in seconds before the first
    /// recurrance of the job. Defaults to no delay (0).</param>
    /// <param name="maximumBacklog">The maximum number of backlogged jobs that
    /// can be scheduled at once to make up for skipped jobs that occurred as a
    /// result of a flood of jobs or due to resource contention.</param>
    /// <param name="jid">The ID of the job. When not provided, an ID will be
    /// generated.</param>
    /// <param name="priority">The priority of the job. Defaults to 0,
    /// representing a low priority. Ignoring other factors like throttles, jobs
    /// with a higher priority value are popped before jobs with a lower
    /// priority.</param>
    /// <param name="retries">The number of retries.</param>
    /// <param name="tags">The tags for the job.</param>
    /// <param name="throttles">The throttles for the job.</param>
    /// <returns>The job ID.</returns>
    Task<string> RecurJobAtIntervalAsync(
        string queueName,
        string className,
        string data,
        int intervalSeconds,
        int initialDelaySeconds = 0,
        int maximumBacklog = 0,
        string? jid = null,
        int priority = 0,
        int retries = 5,
        string[]? tags = null,
        string[]? throttles = null
    );

    /// <summary>
    /// Update the given job to remove the dependency on the job identified by
    /// the given jid.
    /// </summary>
    /// <param name="jid">The ID of the job that the dependency should be
    /// removed from.</param>
    /// <param name="dependsOnJid">The ID of the job that the given job should
    /// not depend on.</param>
    Task<bool> RemoveDependencyFromJobAsync(string jid, string dependsOnJid);

    /// <summary>
    /// Update the given job to remove the given tag.
    /// </summary>
    /// <param name="jid">The ID of the job that the tag should be removed
    /// from</param>
    /// <param name="tag">The tag that should be removed from the job.</param>
    Task<List<string>> RemoveTagFromJobAsync(string jid, string tag);

    /// <summary>
    /// Update the given job to remove the given tags.
    /// </summary>
    /// <param name="jid">The ID of the job that should have tags removed.</param>
    /// <param name="tags">The tags that should be removed to the job.</param>
    Task<List<string>> RemoveTagsFromJobAsync(string jid, params string[] tags);

    /// <summary>
    /// Requeue the given job to the given queue. Similar in functionality to
    /// <see cref="PutJobAsync"/>, but for existing jobs.
    /// </summary>
    /// <param name="workerName">The name of the worker or client that's putting
    /// the job.</param>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="className">The name of the class.</param>
    /// <param name="data">The data to be processed.</param>
    /// <param name="delay">The delay in seconds before the job is
    /// processed. Defaults to no delay (0).</param>
    /// <param name="jid">The ID of the job. When not provided, an ID will be
    /// generated.</param>
    /// <param name="priority">The priority of the job. Defaults to 0,
    /// representing a low priority. Ignoring other factors like throttles, jobs
    /// with a higher priority value are popped before jobs with a lower
    /// priority.</param>
    /// <param name="retries">The number of retries.</param>
    /// <param name="dependencies">The job IDs that this job depends on.</param>
    /// <param name="tags">The tags for the job.</param>
    /// <param name="throttles">The throttles for the job.</param>
    /// <returns>The job ID.</returns>
    Task<string> RequeueJobAsync(
        string workerName,
        string queueName,
        string jid,
        string className,
        string data,
        int delay = 0,
        int? priority = null,
        int? retries = null,
        string[]? dependencies = null,
        string[]? tags = null,
        string[]? throttles = null
    );

    /// <summary>
    /// Enqueue a job to be retried. Similar in functionality to <see
    /// cref="PutJobAsync"/>, but counts against the job's retry limit.
    /// Fails if the given workerName is not the worker with a lock on the job,
    /// or if the job is not currently running.
    /// </summary>
    /// <param name="jid">The job ID.</param>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="workerName">The name of the worker that is requesting to
    /// retry the job.</param>
    /// <param name="group">The group or kind of the failure.</param>
    /// <param name="message">The message for the failure.</param>
    /// <param name="delay">The delay in seconds before the job is retried.</param>
    /// <returns>True if the job was successfully queued to be retried, false if
    /// the job has no more retries remaining.</returns>
    Task<bool> RetryJobAsync(
        string jid,
        string queueName,
        string workerName,
        string group,
        string message,
        int delay = 0
    );

    /// <summary>
    /// Update the priority of the given job.
    /// </summary>
    /// <param name="jid">The ID of the job to update the priority of.</param>
    /// <param name="priority">The new priority value.</param>
    /// <returns>True if the job's priority was successfully updated, otherwise
    /// an exception is raised.</returns>
    Task<bool> SetJobPriorityAsync(string jid, int priority);

    /// <summary>
    /// Timeout a job by its job ID.
    /// </summary>
    /// <param name="jid">The ID of the job that should time out.</param>
    Task TimeoutJobAsync(string jid);

    /// <summary>
    /// Time out one or more jobs by their job IDs.
    /// </summary>
    /// <param name="jids">The IDs of the jobs that should time out.</param>
    Task TimeoutJobsAsync(params string[] jids);

    /// <summary>
    /// Track the job with the given jid.
    /// </summary>
    /// <param name="jid">The ID of the job that should be tracked.</param>
    /// <returns>False if the job is not already tracked, otherwise
    /// true.</returns>
    Task<bool> TrackJobAsync(string jid);

    /// <summary>
    /// Unpause the queue with the given name.
    /// </summary>
    /// <param name="queueName">The name of the queue that should be
    /// unpaused.</param>
    Task UnpauseQueueAsync(string queueName);

    /// <summary>
    /// Untrack the job with the given jid.
    /// </summary>
    /// <param name="jid">The ID of the job that should no longer be
    /// tracked.</param>
    /// <returns>False if the job is not tracked, otherwise true.</returns>
    Task<bool> UntrackJobAsync(string jid);
}