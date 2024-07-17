using Reqless.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Reqless.Client;

/// <summary>
/// A client for interacting with a Reqless-compatible Redish server.
/// </summary>
public class ReqlessClient : IClient, IDisposable
{
    private readonly IRedisExecutor _executor;

    private readonly bool _responsibleForExecutor;

    /// <summary>
    /// Flag tracking whether the instance has been disposed or not. True if the
    /// object has been disposed, false otherwise.
    /// </summary>
    protected bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReqlessClient"/> class. This
    /// constructor is for testing purposes only.
    /// </summary>
    protected ReqlessClient()
    {
        // Forgive null here to silence warning about uninitialized field.
        _executor = null!;
        _responsibleForExecutor = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReqlessClient"/> class from
    /// the given executor. When this constructor is used, the client will not
    /// be responsible for disposing the executor.
    /// </summary>
    /// <param name="executor">The executor to use for executing Reqless
    /// commands.</param>
    public ReqlessClient(IRedisExecutor executor)
    {
        _executor = executor;
        _responsibleForExecutor = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReqlessClient"/> class from a
    /// connection string. The given connection string is used to instantiate a
    /// new <see cref="RedisExecutor"/>. When this constructor is used, the client
    /// will take responsibility for disposing the executor.
    /// </summary>
    /// <param name="connectionString">The connection string to the Redis
    /// server.</param>
    public ReqlessClient(
        string connectionString
    ) : this(new RedisExecutor(connectionString))
    {
        _responsibleForExecutor = true;
    }

    /// <inheritdoc />
    public async Task<bool> AddDependencyToJobAsync(string jid, string dependsOnJid)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));
        ArgumentException.ThrowIfNullOrWhiteSpace(dependsOnJid, nameof(dependsOnJid));

        await _executor.ExecuteAsync([
            "job.addDependency",
            Now(),
            jid,
            dependsOnJid,
        ]);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> AddEventToJobHistoryAsync(
        string jid,
        string what,
        string? data = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));
        ArgumentException.ThrowIfNullOrWhiteSpace(what, nameof(what));

        if (data is not null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));
        }

        var arguments = new RedisValue[4 + (data is null ? 0 : 1)];
        arguments[0] = "job.log";
        arguments[1] = Now();
        arguments[2] = jid;
        arguments[3] = what;
        if (data is not null)
        {
            arguments[4] = data;
        }

        await _executor.ExecuteAsync(arguments);

        return true;
    }

    /// <inheritdoc />
    public async Task<List<string>> AddTagToJobAsync(string jid, string tag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));
        ArgumentException.ThrowIfNullOrWhiteSpace(tag, nameof(tag));

        return await AddTagsToJobAsync(jid, tag);
    }

    /// <inheritdoc />
    public async Task<List<string>> AddTagsToJobAsync(string jid, params string[] tags)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));
        ArgumentNullException.ThrowIfNull(tags, nameof(tags));

        var arguments = new RedisValue[tags.Length + 3];
        arguments[0] = "job.addTag";
        arguments[1] = Now();
        arguments[2] = jid;
        CopyStringArguments(tags, ref arguments, 3, nameof(tags));

        var result = await _executor.ExecuteAsync(arguments);

        var tagsJson = (string?)result
            ?? throw new InvalidOperationException(
                "Server returned unexpected null result."
            );

        var resultTags = JsonSerializer.Deserialize<List<string>>(tagsJson)
            ?? throw new JsonException($"Failed to deserialize tags JSON: {tagsJson}");

        return resultTags;
    }

    /// <inheritdoc />
    public async Task<bool> CancelJobAsync(string jid)
    {
        return await CancelJobsAsync(jid);
    }

    /// <inheritdoc />
    public async Task<bool> CancelJobsAsync(params string[] jids)
    {
        ArgumentNullException.ThrowIfNull(jids, nameof(jids));

        if (jids.Length == 0)
        {
            return true;
        }

        var arguments = new RedisValue[jids.Length + 2];
        arguments[0] = "job.cancel";
        arguments[1] = Now();
        CopyStringArguments(jids, ref arguments, 2, nameof(jids));
        await _executor.ExecuteAsync(arguments);
        // If no error occurred, the jobs were either cancelled or didn't exist.
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CompleteJobAsync(
        string jid,
        string workerName,
        string queueName,
        string data
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));
        ArgumentException.ThrowIfNullOrWhiteSpace(workerName, nameof(workerName));
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName, nameof(queueName));
        ArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));

        var arguments = new RedisValue[] {
            "job.complete",
            Now(),
            jid,
            workerName,
            queueName,
            data,
        };
        await _executor.ExecuteAsync(arguments);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CompleteAndRequeueJobAsync(
        string jid,
        string workerName,
        string queueName,
        string data,
        string nextQueueName,
        int delay = 0,
        string[]? dependencies = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));
        ArgumentException.ThrowIfNullOrWhiteSpace(workerName, nameof(workerName));
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName, nameof(queueName));
        ArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));

        var _dependencies = dependencies ?? [];
        var arguments = new RedisValue[] {
            "job.completeAndRequeue",
            Now(),
            jid,
            workerName,
            queueName,
            data,
            nextQueueName,
            "delay",
            delay,
            "depends",
            JsonSerializer.Serialize(_dependencies),
        };

        await _executor.ExecuteAsync(arguments);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> FailJobAsync(
        string jid,
        string workerName,
        string group,
        string message,
        string? data = null
    )
    {
        var arguments = new RedisValue[6 + (data == null ? 0 : 1)];
        arguments[0] = "job.fail";
        arguments[1] = Now();
        arguments[2] = jid;
        arguments[3] = workerName;
        arguments[4] = group;
        arguments[5] = message;
        if (data != null)
        {
            arguments[6] = data;
        }
        await _executor.ExecuteAsync(arguments);

        // If no error occurred, the job was failed successfully.
        return true;
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, int>> FailureGroupsCountsAsync()
    {
        RedisResult result = await _executor.ExecuteAsync(
            ["failureGroups.counts", Now()]
        );

        var failedCountsJson = (string?)result
            ?? throw new InvalidOperationException(
                "Server returned unexpected null result."
            );

        var keyValues = JsonSerializer.Deserialize<Dictionary<string, int>>(
            failedCountsJson
        ) ?? throw new JsonException(
            $"Failed to deserialize failed counts JSON: {failedCountsJson}"
        );

        return keyValues;
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, JsonElement>> GetAllConfigsAsync()
    {
        var result = await _executor.ExecuteAsync(["config.getAll", Now()]);

        var configsJson = (string?)result
            ?? throw new InvalidOperationException(
                "Server returned unexpected null result."
            );

        var configs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
            configsJson
        ) ?? throw new JsonException($"Failed to deserialize config JSON: {result}");

        return configs;
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetCompletedJobsAsync(
        int limit = 25,
        int offset = 0
    )
    {
        var result = await _executor.ExecuteAsync([
            "jobs.completed",
            Now(),
            offset,
            limit
        ]);

        return ValidateJidsResult(result);
    }

    /// <inheritdoc/>
    public async Task<Job?> GetJobAsync(string jid)
    {
        RedisResult result = await _executor.ExecuteAsync(["job.get", Now(), jid]);

        var jobJson = (string?)result;

        if (jobJson == null)
        {
            return null;
        }

        var job = JsonSerializer.Deserialize<Job>(jobJson)
          ?? throw new JsonException($"Failed to deserialize job JSON: {jobJson}");

        return job;
    }

    /// <inheritdoc/>
    public async Task<Job[]> GetJobsAsync(params string[] jids)
    {
        ArgumentNullException.ThrowIfNull(jids, nameof(jids));

        var arguments = new RedisValue[jids.Length + 2];
        arguments[0] = "job.getMulti";
        arguments[1] = Now();
        CopyStringArguments(jids, ref arguments, 2, nameof(jids));

        RedisResult result = await _executor.ExecuteAsync(arguments);
        var jobsJson = (string?)result
            ?? throw new InvalidOperationException(
                "Server returned unexpected null result."
            );

        // Redis cjson can't distinguish between an empty array and an empty
        // object, so an empty object here actually represents an empty
        // array, and, ergo, no jobs retrieved.
        if (jobsJson == "{}")
        {
            return [];
        }

        var jobs = JsonSerializer.Deserialize<Job[]>(jobsJson)
          ?? throw new JsonException($"Failed to deserialize job JSON: {jobsJson}");

        return jobs;
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetJobsByStateAsync(
        string state,
        string queueName,
        int limit = 25,
        int offset = 0
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(state, nameof(state));
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName, nameof(queueName));

        var result = await _executor.ExecuteAsync([
            "queue.jobsByState",
            Now(),
            state,
            queueName,
            offset,
            limit,
        ]);

        if (result.IsNull)
        {
            throw new InvalidOperationException(
                "Server returned unexpected null result."
            );
        }

        // For whatever reason, if result is null, this cast results in
        // string?[] { null }, so we check for null directly above and forgive
        // null here.
        var jidsResult = ((string?[]?)result)!;

        List<string> jids = jidsResult.Select(jid =>
            jid ?? throw new InvalidOperationException(
                "Server returned unexpected null jid."
            )
        ).ToList();

        return jids;
    }

    /// <inheritdoc/>
    public async Task<int> GetQueueLengthAsync(string queueName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName, nameof(queueName));
        var result = await _executor.ExecuteAsync(["queue.length", Now(), queueName]);

        var length = (int?)result
            ?? throw new InvalidOperationException(
                "Server returned unexpected null result."
            );

        return length;
    }

    /// <inheritdoc/>
    public async Task<long> HeartbeatJobAsync(
        string jid,
        string workerName,
        string? data = null
    )
    {
        var arguments = new RedisValue[4 + (data == null ? 0 : 1)];
        arguments[0] = "job.heartbeat";
        arguments[1] = Now();
        arguments[2] = jid;
        arguments[3] = workerName;
        if (data != null)
        {
            arguments[4] = data;
        }

        var result = await _executor.ExecuteAsync(arguments);

        long newExpires = (long?)result
            ?? throw new InvalidOperationException(
                "Server returned unexpected null result."
            );

        return newExpires;
    }

    /// <inheritdoc/>
    public async Task<Job?> PopJobAsync(string queueName, string workerName)
    {
        var jobs = await PopJobsAsync(queueName, workerName, 1);
        return jobs.Count == 0 ? null : jobs[0];
    }

    /// <inheritdoc/>
    public async Task<List<Job>> PopJobsAsync(
        string queueName,
        string workerName,
        int limit
    )
    {
        RedisResult result = await _executor.ExecuteAsync([
            "queue.pop",
            Now(),
            queueName,
            workerName,
            limit
        ]);
        var jobsJson = (string?)result
            ?? throw new InvalidOperationException(
                "Server returned unexpected null result."
            );

        // Redis cjson can't distinguish between an empty array and an empty
        // object, so an empty object here actually represents an empty
        // array, and, ergo, no jobs retrieved.
        if (jobsJson == "{}")
        {
            return [];
        }

        var jobs = JsonSerializer.Deserialize<List<Job>>(jobsJson)
            ?? throw new JsonException(
                $"Failed to deserialize jobs JSON: {jobsJson}"
            );

        return jobs;
    }

    /// <inheritdoc/>
    public async Task<string> PutJobAsync(
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
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workerName, nameof(workerName));
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName, nameof(queueName));
        ArgumentException.ThrowIfNullOrWhiteSpace(className, nameof(className));
        ArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));

        var _jid = jid ?? Guid.NewGuid().ToString("N");
        var _dependencies = dependencies ?? [];
        var _tags = tags ?? [];
        var _throttles = throttles ?? [];
        RedisResult result = await _executor.ExecuteAsync([
            "queue.put",
            Now(),
            workerName,
            queueName,
            _jid,
            className,
            data,
            delay,
            "depends",
            JsonSerializer.Serialize(_dependencies),
            "priority",
            priority,
            "retries",
            retries,
            "tags",
            JsonSerializer.Serialize(_tags),
            "throttles",
            JsonSerializer.Serialize(_throttles),
        ]);

        var resultJid = (string?)result
            ?? throw new InvalidOperationException(
                "Server returned unexpected null result."
            );

        return resultJid;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveDependencyFromJobAsync(string jid, string dependsOnJid)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));
        ArgumentException.ThrowIfNullOrWhiteSpace(dependsOnJid, nameof(dependsOnJid));

        await _executor.ExecuteAsync([
            "job.removeDependency",
            Now(),
            jid,
            dependsOnJid,
        ]);

        return true;
    }

    /// <inheritdoc />
    public async Task<List<string>> RemoveTagFromJobAsync(string jid, string tag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));
        ArgumentException.ThrowIfNullOrWhiteSpace(tag, nameof(tag));

        return await RemoveTagsFromJobAsync(jid, tag);
    }

    /// <inheritdoc />
    public async Task<List<string>> RemoveTagsFromJobAsync(string jid, params string[] tags)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));
        ArgumentNullException.ThrowIfNull(tags, nameof(tags));

        var arguments = new RedisValue[tags.Length + 3];
        arguments[0] = "job.removeTag";
        arguments[1] = Now();
        arguments[2] = jid;
        CopyStringArguments(tags, ref arguments, 3, nameof(tags));

        var result = await _executor.ExecuteAsync(arguments);

        var tagsJson = (string?)result
            ?? throw new InvalidOperationException(
                "Server returned unexpected null result."
            );

        var resultTags = JsonSerializer.Deserialize<List<string>>(tagsJson)
            ?? throw new JsonException($"Failed to deserialize tags JSON: {tagsJson}");

        return resultTags;
    }

    /// <inheritdoc />
    public async Task<string> RequeueJobAsync(
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
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workerName, nameof(workerName));
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName, nameof(queueName));
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));
        ArgumentException.ThrowIfNullOrWhiteSpace(className, nameof(className));
        ArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));

        RedisValue _dependencies = dependencies is not null
            ? JsonSerializer.Serialize(dependencies)
            : RedisValue.Null;
        RedisValue _tags = tags is not null
            ? JsonSerializer.Serialize(tags)
            : RedisValue.Null;
        RedisValue _throttles = throttles is not null
            ? JsonSerializer.Serialize(throttles)
            : RedisValue.Null;

        await _executor.ExecuteAsync([
            "job.requeue",
            Now(),
            workerName,
            queueName,
            jid,
            className,
            data,
            delay,
            "priority",
            priority ?? RedisValue.Null,
            "retries",
            retries ?? RedisValue.Null,
            "depends",
            _dependencies,
            "tags",
            _tags,
            "throttles",
            _throttles,
        ]);

        return jid;
    }

    /// <inheritdoc/>
    public async Task<bool> RetryJobAsync(
        string jid,
        string queueName,
        string workerName,
        string group,
        string message,
        int delay = 0
    )
    {
        RedisResult result = await _executor.ExecuteAsync([
            "job.retry",
            Now(),
            jid,
            queueName,
            workerName,
            delay,
            group,
            message,
        ]);

        var remainingRetries = (int?)result
            ?? throw new InvalidOperationException(
                "Server returned unexpected null result."
            );

        return remainingRetries > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> SetJobPriorityAsync(string jid, int priority)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));

        await _executor.ExecuteAsync([
            "job.setPriority",
            Now(),
            jid,
            priority,
        ]);

        return true;
    }

    /// <inheritdoc/>
    public async Task TimeoutJobAsync(string jid)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));
        await TimeoutJobsAsync(jid);
    }

    /// <inheritdoc/>
    public async Task TimeoutJobsAsync(params string[] jids)
    {
        var arguments = new RedisValue[jids.Length + 2];
        arguments[0] = "job.timeout";
        arguments[1] = Now();
        CopyStringArguments(jids, ref arguments, 2, nameof(jids));
        await _executor.ExecuteAsync(arguments);
    }

    /// <inheritdoc/>
    public async Task<bool> TrackJobAsync(string jid)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));

        var result = await _executor.ExecuteAsync([
            "job.track",
            Now(),
            jid,
        ]);

        var addedCount = (int?)result ?? throw new InvalidOperationException(
            "Server returned unexpected null result."
        );

        return addedCount == 1;
    }

    /// <inheritdoc/>
    public async Task<bool> UntrackJobAsync(string jid)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));

        var result = await _executor.ExecuteAsync([
            "job.untrack",
            Now(),
            jid,
        ]);

        var removedCount = (int?)result ?? throw new InvalidOperationException(
            "Server returned unexpected null result."
        );

        return removedCount == 1;
    }

    /// <summary>
    /// Gets the current time in milliseconds since the Unix epoch for use as
    /// the now argument when invoking qless commands.
    /// </summary>
    /// <remarks>
    /// This method is virtual to allow for easy mocking in tests.
    /// </remarks>
    protected virtual long Now()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Copies a range of elements from a source array of strings and pastes
    /// them into an array of RedisValue starting at the specified destination
    /// index.
    /// </summary>
    /// <remarks>
    /// Array.Copy doesn't work in these scenarios, so copying manually.
    /// </remarks>
    /// <param name="source">The array of strings to copy from.</param>
    /// <param name="destination">The array of RedisValue to copy to.</param>
    /// <param name="destinationIndex">The index in the destination array at
    /// which to start copying.</param>
    /// <param name="argumentName">The name of the argument being copied; for
    /// error reporting purposes.</param>
    protected static void CopyStringArguments(
        string[] source,
        ref RedisValue[] destination,
        int destinationIndex,
        string argumentName
    )
    {
        for (var index = 0; index < source.Length; index++)
        {
            var value = source[index];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    "Value cannot include null, empty string, or strings composed entirely of whitespace.",
                    argumentName
                );
            }

            destination[index + destinationIndex] = value;
        }
    }

    /// <summary>
    /// Validates the result of a command that returns a list of JIDs.
    /// </summary>
    /// <param name="result">The redis result that is expected to be a list of
    /// job IDs.</param>
    /// <returns>A list of the job IDs.</returns>
    /// <exception cref="InvalidOperationException">If the server returns
    /// null.</exception>
    protected static List<string> ValidateJidsResult(RedisResult result)
    {
        if (result.IsNull)
        {
            throw new InvalidOperationException(
                "Server returned unexpected null result."
            );
        }

        // For whatever reason, if result is null, this cast results in
        // string?[] { null }, so we check for null directly above and forgive
        // null here.
        var jidsResult = ((string?[]?)result)!;

        List<string> jids = jidsResult.Select(jid =>
            jid ?? throw new InvalidOperationException(
                "Server returned unexpected null jid."
            )
        ).ToList();

        return jids;
    }

    /// <summary>
    /// Releases all resources used by the <see cref="ReqlessClient"/>.
    /// </summary>
    public virtual void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases all resources used by the <see cref="ReqlessClient"/>.
    /// </summary>
    /// <param name="disposing">True if disposing, false otherwise.</param>
    protected virtual void Dispose(bool disposing)
    {
        lock (this)
        {
            if (!_disposed && disposing)
            {
                _disposed = true;
                if (_responsibleForExecutor && _executor is IDisposable executor)
                {
                    executor.Dispose();
                }
            }
        }
    }
}