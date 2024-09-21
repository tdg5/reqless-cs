using Reqless.Client.Models;

namespace Reqless.Client;

/// <summary>
/// Interface for a client that can interact with a Redish-backed Reqless
/// server.
/// </summary>
public interface IReqlessClient
{
    /// <summary>
    /// Update the given job to add a dependency on the job identified by the
    /// given jid.
    /// </summary>
    /// <param name="jid">The ID of the job that should depend on the given
    /// jid.</param>
    /// <param name="dependsOnJid">The ID of the job that the given job should
    /// depend on.</param>
    /// <returns>True if the dependency was successfully added, otherwise an
    /// exception is raised.</returns>
    Task<bool> AddDependencyToJobAsync(string jid, string dependsOnJid);

    /// <summary>
    /// Add an event to the history of the job with the given jid.
    /// </summary>
    /// <param name="jid">The ID of the job to add an event to.</param>
    /// <param name="what">The what or name for the event.</param>
    /// <param name="data">Optional data for the event.</param>
    /// <returns>True if the event was successfully added to the job's history,
    /// otherwise an exception is raised.</returns>
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
    /// <returns>The list of tags that are now on the job.</returns>
    Task<List<string>> AddTagToJobAsync(string jid, string tag);

    /// <summary>
    /// Update the given job to add the given tags.
    /// </summary>
    /// <param name="jid">The ID of the job that should have tags added.</param>
    /// <param name="tags">The tags that should be added to the job.</param>
    /// <returns>The list of tags that are now on the job.</returns>
    Task<List<string>> AddTagsToJobAsync(string jid, params string[] tags);

    /// <summary>
    /// Update the given recurring job to add the given tag.
    /// </summary>
    /// <param name="jid">The ID of the recurring job that the tag should be added
    /// to.</param>
    /// <param name="tag">The tag that should be added to the recurring
    /// job.</param>
    /// <returns>The list of tags that are now on the recurring job.</returns>
    Task<List<string>> AddTagToRecurringJobAsync(string jid, string tag);

    /// <summary>
    /// Update the given recurring job to add the given tags.
    /// </summary>
    /// <param name="jid">The ID of the recurring job that should have tags
    /// added.</param>
    /// <param name="tags">The tags that should be added to the recurring
    /// job.</param>
    /// <returns>The list of tags that are now on the recurring job.</returns>
    Task<List<string>> AddTagsToRecurringJobAsync(string jid, params string[] tags);

    /// <summary>
    /// Cancel a job.
    /// </summary>
    /// <param name="jid">The ID of the job that should be cancelled.</param>
    /// <returns>Returns true if the job was successfully cancelled, otherwise
    /// an exception is raised.</returns>
    Task<bool> CancelJobAsync(string jid);

    /// <summary>
    /// Cancel one or more jobs.
    /// </summary>
    /// <param name="jids">The IDs of the jobs that should be cancelled.</param>
    /// <returns>Returns true if the jobs were successfully cancelled, otherwise
    /// an exception is raised.</returns>
    Task<bool> CancelJobsAsync(params string[] jids);

    /// <summary>
    /// Cancel a recurring job.
    /// </summary>
    /// <param name="jid">The ID of the recurring job for which all futurue
    /// occurences should be cancelled.</param>
    Task CancelRecurringJobAsync(string jid);

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
    /// Delete the throttle with the given name.
    /// </summary>
    /// <param name="throttleName">The name of the throttle that should be
    /// deleted.</param>
    Task DeleteThrottleAsync(string throttleName);

    /// <summary>
    /// Fails a job.
    /// </summary>
    /// <param name="jid">The job ID.</param>
    /// <param name="workerName">The name of the worker that failed the job.</param>
    /// <param name="groupName">The group name or kind of the failure.</param>
    /// <param name="message">The message for the failure.</param>
    /// <param name="data">Optional, updated data for the job.</param>
    /// <returns>True if the job was successfully failed, otherwise an exception
    /// is raised.</returns>
    Task<bool> FailJobAsync(
        string jid,
        string workerName,
        string groupName,
        string message,
        string? data = null
    );

    /// <summary>
    /// Forget the config with the given name.
    /// </summary>
    /// <param name="configName">The name of the config that should be forgotten.</param>
    Task ForgetConfigAsync(string configName);

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
    /// Forget the worker with the given name.
    /// </summary>
    /// <param name="workerName">The name of the worker that should be removed
    /// from the set of known worker.</param>
    Task ForgetWorkerAsync(string workerName);

    /// <summary>
    /// Forget the workers with the given names.
    /// </summary>
    /// <param name="workerNames">The names of workers that should be removed
    /// from the set of known workers.</param>
    Task ForgetWorkersAsync(params string[] workerNames);

    /// <summary>
    /// Get all Reqless configuration values.
    /// </summary>
    /// <returns>A dictionary where each key is the name of a config and each
    /// value is the respective value.</returns>
    Task<Dictionary<string, string>> GetAllConfigsAsync();

    /// <summary>
    /// Get the counts of the number of jobs in various states for all queues.
    /// </summary>
    /// <returns>A <see cref="List{QueueCounts}"/> for each of the known
    /// queues.</returns>
    Task<List<QueueCounts>> GetAllQueueCountsAsync();

    /// <summary>
    /// Get all dynamic queue identifier patterns.
    /// </summary>
    /// <returns>A dictionary where each key is a queue identifier and each
    /// value is a list of queues and patterns that map to that
    /// identifier.</returns>
    Task<Dictionary<string, List<string>>> GetAllQueueIdentifierPatternsAsync();

    /// <summary>
    /// Get the names of all known queues.
    /// </summary>
    /// <returns>A list of the names of all known queues.</returns>
    Task<List<string>> GetAllQueueNamesAsync();

    /// <summary>
    /// Get all dynamic queue priority patterns.
    /// </summary>
    /// <returns>An ordered collection of <see cref="QueuePriorityPattern"/>
    /// where each element represents a priority class and those queues that are
    /// part of that priority class.</returns>
    Task<List<QueuePriorityPattern>> GetAllQueuePriorityPatternsAsync();

    /// <summary>
    /// Get the counts of the number of jobs, both expired and unexpired, that
    /// each worker is responsible for.
    /// </summary>
    /// <returns>A <see cref="List{WorkerCounts}"/> for each of the known
    /// workers.</returns>
    Task<List<WorkerCounts>> GetAllWorkerCountsAsync();

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
    /// Retrieve the value of the config with the given name, if such a config
    /// is defined.
    /// </summary>
    /// <param name="configName"></param>
    /// <returns>The config value if it is defined.</returns>
    Task<string?> GetConfigAsync(string configName);

    /// <summary>
    /// Gets the IDs of jobs that have failed with the given failure group name.
    /// </summary>
    /// <param name="groupName">The name of the failure group to retrieve job
    /// failures for.</param>
    /// <param name="limit">The maximum number of job IDs to retrieve.</param>
    /// <param name="offset">The number of job IDs to skip before returning
    /// results.</param>
    /// <returns>A <see cref="JidsResult"/> containing the job IDs of failed
    /// jobs in the given group.</returns>
    Task<JidsResult> GetFailedJobsByGroupAsync(
        string groupName,
        int limit = 25,
        int offset = 0
    );

    /// <summary>
    /// Returns a dictionary where each key is a known failure group name and
    /// each value is the count of jobs that have failed with that group.
    /// </summary>
    /// <returns>A dictionary where each key is a known failure group name and
    /// each value is the count of jobs that have failed with that
    /// group.</returns>
    Task<Dictionary<string, int>> GetFailureGroupsCountsAsync();

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
    /// <returns>An list of the retrieved jobs.</returns>
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
    /// <returns>A <see cref="JidsResult"/> containing the job IDs of jobs
    /// with the given tag.</returns>
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
    /// <returns>A <see cref="QueueCounts"/> with defailts of the counts of jobs
    /// in various states in the given queue.</returns>
    Task<QueueCounts> GetQueueCountsAsync(string queueName);

    /// <summary>
    /// Get the count of jobs in the given queue.
    /// </summary>
    /// <param name="queueName">The name of the queue to get the length of.</param>
    /// <returns>The lengh of the queue.</returns>
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
    /// <returns>A <see cref="QueueStats"/> instance with the stats for the
    /// given queue on the given date.</returns>
    Task<QueueStats> GetQueueStatsAsync(string queueName, DateTimeOffset? date);

    /// <summary>
    /// Get the throttle for the queue with the given name.
    /// </summary>
    /// <param name="queueName">The name of the queue to retrieve the throttle
    /// for.</param>
    /// <returns>A <see cref="Throttle"/> instance corresponding to the given
    /// queue.</returns>
    Task<Throttle> GetQueueThrottleAsync(string queueName);

    /// <summary>
    /// Gets a recurring job by its job ID.
    /// </summary>
    /// <param name="jid">The ID of the recurring job that should be
    /// retreived.</param>
    /// <returns>The recurring job, if it exists; otherwise, null.</returns>
    Task<RecurringJob?> GetRecurringJobAsync(string jid);

    /// <summary>
    /// Get the throttle with the given name.
    /// </summary>
    /// <param name="throttleName">The name of the throttle to retrieve.</param>
    /// <returns>A <see cref="Throttle"/> instance corresponding to the throttle
    /// with the given name.</returns>
    Task<Throttle> GetThrottleAsync(string throttleName);

    /// <summary>
    /// Get the list of job IDs that currently hold locks for the given throttle.
    /// </summary>
    /// <param name="throttleName">The name of the throttle for which lock
    /// owners should be retieved.</param>
    /// <returns>The list of job IDs that currently own locks for the given
    /// throttle.</returns>
    Task<List<string>> GetThrottleLockOwnersAsync(string throttleName);

    /// <summary>
    /// Get the list of job IDs that are currently waiting for locks for the
    /// given throttle.
    /// </summary>
    /// <param name="throttleName">The name of the throttle for which lock
    /// waiters should be retieved.</param>
    /// <returns>The list of job IDs that are currently waiting for locks for
    /// the given throttle.</returns>
    Task<List<string>> GetThrottleLockWaitersAsync(string throttleName);

    /// <summary>
    /// Get the most commonly used tags in order from most used to least used.
    /// </summary>
    /// <remarks>
    /// Tags with only a single usage are ignored.
    /// </remarks>
    /// <param name="offset">The number of tags to skip before returning
    /// results.</param>
    /// <param name="limit">The maximum number of tags to retrieve.</param>
    /// <returns>A list of any matching tags.</returns>
    Task<List<string>> GetTopTagsAsync(int limit = 25, int offset = 0);

    /// <summary>
    /// Gets the details of all currently tracked jobs.
    /// </summary>
    /// <returns>A <see cref="TrackedJobsResult"/> instance containing the
    /// details of all currently tracked jobs.</returns>
    Task<TrackedJobsResult> GetTrackedJobsAsync();

    /// <summary>
    /// Get the IDs of jobs, both expired and unexpired, that are the
    /// responsibility of the worker with the given name.
    /// </summary>
    /// <param name="workerName">The name of the worker to retrieve jobs
    /// for.</param>
    /// <returns>A <see cref="WorkerJobs"/> instance containing the details of
    /// the jobs that are the responsibility of the worker with the given
    /// name.</returns>
    Task<WorkerJobs> GetWorkerJobsAsync(string workerName);

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
    /// <returns>A <see cref="List{Job}"/> with the details of zero or more
    /// jobs.</returns>
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
    /// <returns>A <see cref="List{Job}"/> of the jobs that were popped, if
    /// any.</returns>
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
    /// Release any pending or owned locks for the throttle with the given name
    /// that are owned by the job with the given ID.
    /// </summary>
    /// <param name="jid">The ID of job that should release any pending or owned
    /// locks for the given throttle.</param>
    /// <param name="throttleName">The name of the throttle that the jobs should
    /// no longer be constrained by.</param>
    Task ReleaseJobThrottleAsync(string jid, string throttleName);

    /// <summary>
    /// Release any pending or owned locks for the throttle with the given name
    /// that are owned by the jobs with the given job IDs.
    /// </summary>
    /// <param name="throttleName">The name of the throttle for which pending or
    /// owned locks should be released.</param>
    /// <param name="jids">The IDs of jobs that should release any pending or
    /// owned locks for the given throttle.</param>
    Task ReleaseThrottleForJobsAsync(string throttleName, params string[] jids);

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
    /// <returns>A list of the tags that remain on the job.</returns>
    Task<List<string>> RemoveTagFromJobAsync(string jid, string tag);

    /// <summary>
    /// Update the given job to remove the given tags.
    /// </summary>
    /// <param name="jid">The ID of the job that should have tags removed.</param>
    /// <param name="tags">The tags that should be removed from the job.</param>
    /// <returns>A list of the tags that remain on the job.</returns>
    Task<List<string>> RemoveTagsFromJobAsync(string jid, params string[] tags);

    /// <summary>
    /// Update the given recurring job to remove the given tag.
    /// </summary>
    /// <param name="jid">The ID of the recurring job that the tag should be
    /// removed from</param>
    /// <param name="tag">The tag that should be removed from the recurring
    /// job.</param>
    /// <returns>A list of the tags that remain on the remaining job.</returns>
    Task<List<string>> RemoveTagFromRecurringJobAsync(string jid, string tag);

    /// <summary>
    /// Update the given recurring job to remove the given tags.
    /// </summary>
    /// <param name="jid">The ID of the recurring job that should have tags
    /// removed.</param>
    /// <param name="tags">The tags that should be removed from the recurring
    /// job.</param>
    /// <returns>A list of the tags that remain on the remaining job.</returns>
    Task<List<string>> RemoveTagsFromRecurringJobAsync(string jid, params string[] tags);

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
    /// <param name="groupName">The group or kind of the failure.</param>
    /// <param name="message">The message for the failure.</param>
    /// <param name="delay">The delay in seconds before the job is retried.</param>
    /// <returns>True if the job was successfully queued to be retried, false if
    /// the job has no more retries remaining.</returns>
    Task<bool> RetryJobAsync(
        string jid,
        string queueName,
        string workerName,
        string groupName,
        string message,
        int delay = 0
    );

    /// <summary>
    /// Set all dynamic queue identifier patterns.
    /// </summary>
    /// <param name="identifierPatterns">A dictionary where each key is a queue
    /// identifier and each value is a list of queues and patterns that should
    /// map to that identifier.</param>
    Task SetAllQueueIdentifierPatternsAsync(
        Dictionary<string, IEnumerable<string>> identifierPatterns
    );

    /// <summary>
    /// Set all dynamic queue priority patterns.
    /// </summary>
    /// <param name="priorityPatterns">A collection of <see
    /// cref="QueuePriorityPattern"/> that should replace any existing
    /// records.</param>
    Task SetAllQueuePriorityPatternsAsync(
        IEnumerable<QueuePriorityPattern> priorityPatterns
    );

    /// <summary>
    /// Set the config with the given name to the given value.
    /// </summary>
    /// <param name="configName">The name that should be used to store the config.</param>
    /// <param name="value">The value that should be stored.</param>
    Task SetConfigAsync(string configName, string value);

    /// <summary>
    /// Update the priority of the given job.
    /// </summary>
    /// <param name="jid">The ID of the job to update the priority of.</param>
    /// <param name="priority">The new priority value.</param>
    /// <returns>True if the job's priority was successfully updated, otherwise
    /// an exception is raised.</returns>
    Task<bool> SetJobPriorityAsync(string jid, int priority);

    /// <summary>
    /// Set the maximum for the throttle associated with the queue with the
    /// given name.
    /// </summary>
    /// <param name="queueName">The name of the queue to modify the throttle
    /// for.</param>
    /// <param name="maximum">The new maximum value for the throttle.</param>
    Task SetQueueThrottleAsync(string queueName, int maximum);

    /// <summary>
    /// Set the maximum for the throttle with the given name.
    /// </summary>
    /// <param name="throttleName">The name of the throttle to modify.</param>
    /// <param name="maximum">The new maximum value for the throttle.</param>
    /// <param name="ttl">The time-to-live for the throttle. By default doesn't
    /// modify the throttle's TTL. Any value less than zero doesn't mutate the
    /// TTL.</param>
    Task SetThrottleAsync(string throttleName, int maximum, int ttl = 0);

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
    /// Unfail count number of jobs from the given failure group into the given
    /// queue.
    /// </summary>
    /// <param name="queueName">The name of the queue which should receive the
    /// unfailed jobs.</param>
    /// <param name="groupName">The name of the failure group from which jobs
    /// should be taken.</param>
    /// <param name="count">The maximum number of jobs that should be
    /// unfailed.</param>
    /// <returns>The number of jobs that were unfaild.</returns>
    Task<int> UnfailJobsFromFailureGroupIntoQueueAsync(
        string queueName,
        string groupName,
        int count = 25
    );

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

    /// <summary>
    /// Update the given recurring job.
    /// </summary>
    /// <param name="jid">The ID of the recurring job to update.</param>
    /// <param name="className">The new class name for the recurring job, if
    /// any.</param>
    /// <param name="data">The new data for the recurring job, if any.</param>
    /// <param name="intervalSeconds">The new interval, in seconds, for the
    /// recurring job, if any.</param>
    /// <param name="maximumBacklog">The new maximum backlog for the recurring
    /// job, if any.</param>
    /// <param name="priority">The new priority for the recurring job, if
    /// any.</param>
    /// <param name="queueName">The new queue for the recurring job, if
    /// any.</param>
    /// <param name="retries">The new number of retries for the recurring job,
    /// if any.</param>
    /// <param name="throttles">The new throttles for the recurring job, if
    /// any.</param>
    Task UpdateRecurringJobAsync(
        string jid,
        string? className = null,
        string? data = null,
        int? intervalSeconds = null,
        int? maximumBacklog = null,
        int? priority = null,
        string? queueName = null,
        int? retries = null,
        string[]? throttles = null
    );
}