using Reqless.Models;
using StackExchange.Redis;

namespace Reqless;

/// <summary>
/// Interface for a client that can interact with a Redish-backed Reqless
/// server.
/// </summary>
public interface IClient
{
    // bool Cancel(string jid);

    // string[] Cancel(string[] jids);

    // bool Complete(
    //     string jid,
    //     string workerName,
    //     string queueName,
    //     string data,
    //     string? nextQueueName = null,
    //     int delay = 0,
    //     string[]? depends = null
    // );

    // bool Track(string jid);

    // bool Untrack(string jid);

    // RedisValue ?
    // Dictionary<string, string> GetConfig();

    // RedisValue ?
    // void SetConfig(string name, string value);

    // void UnsetConfig(string name);

    // bool Depends(string jid, string[] jids);

    /// <summary>
    /// Cancel a job.
    /// </summary>
    /// <param name="jid">The job ID.</param>
    Task<bool> CancelJobAsync(string jid);

    /// <summary>
    /// Fails a job.
    /// </summary>
    /// <param name="jid">The job ID.</param>
    /// <param name="workerName">The name of the worker that failed the job.</param>
    /// <param name="group">The group or kind of the failure.</param>
    /// <param name="message">The message for the failure.</param>
    /// <param name="data">Optional, updated data for the job.</param>
    Task<bool> FailAsync(
        string jid,
        string workerName,
        string group,
        string message,
        string? data = null
    );

    /// <summary>
    /// Returns the counts of the various groups of failures known.
    /// </summary>
    Task<Dictionary<string, int>> FailedCountsAsync();

    /// <summary>
    /// Gets a job by its job ID.
    /// </summary>
    /// <param name="jid">The job ID.</param>
    /// <returns>The job, if it exists; otherwise, null.</returns>
    Task<Job?> GetJobAsync(string jid);

    /// <summary>
    /// Attempt to pop a job from a queue.
    /// </summary>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="workerName">The name of the worker that is popping the
    /// job.</param>
    /// <returns>The job, if one was popped; otherwise, null.</returns>
    public Task<Job?> PopAsync(string queueName, string workerName);

    /// <summary>
    /// Attempt to pop multiple jobs from a queue.
    /// </summary>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="workerName">The name of the worker that is popping the
    /// jobs.</param>
    /// <param name="count">The maximum number of jobs to pop.</param>
    /// <returns>The jobs that were popped, if any.</returns>
    public Task<List<Job>> PopMultiAsync(
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
    /// processed.</param>
    /// <param name="jid">The job ID.</param>
    /// <param name="priority">The priority of the job.</param>
    /// <param name="retries">The number of retries.</param>
    /// <param name="dependencies">The job IDs that this job depends on.</param>
    /// <param name="tags">The tags for the job.</param>
    /// <param name="throttles">The throttles for the job.</param>
    /// <returns>The job ID.</returns>
    Task<string> PutAsync(
        string workerName,
        string queueName,
        string className,
        string data,
        int delay = 0,
        string? jid = null,
        int priority = 0,
        int retries = 0,
        string[]? dependencies = null,
        string[]? tags = null,
        string[]? throttles = null
    );

    /// <summary>
    /// Enqueue a job to be retried. Similar in functionality to <see
    /// cref="PutAsync"/>, but counts against the job's retry limit. 
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
    public Task<bool> RetryAsync(
        string jid,
        string queueName,
        string workerName,
        string group,
        string message,
        int delay = 0
    );

    // failed
    // get
    // heartbeat
    // jobs
    // length
    // multiget
    // pause
    // peek
    // pop
    // priority
    // queues
    // recur
    // recur.get
    // recur.tag
    // recur.untag
    // recur.update
    // requeue
    // stats
    // tag
    // throttle.delete
    // throttle.get
    // throttle.locks
    // throttle.pending
    // throttle.set
    // throttle.ttl
    // timeout
    // track
    // unpause
    // unrecur
    // workers
}