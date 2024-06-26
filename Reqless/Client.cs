using Reqless.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Reqless;

/// <summary>
/// A client for interacting with a Qless-compatible Redish server.
/// </summary>
public class Client : IClient, IDisposable
{
    private readonly static string _luaScript = ResourceHelper.ReadTextResource("lua/qless.lua");

    private readonly ConnectionMultiplexer _connection;

    /// <summary>
    /// Flag tracking whether the instance has been disposed or not. True if the
    /// object has been disposed, false otherwise.
    /// </summary>
    protected bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class. This
    /// constructor is for testing purposes only.
    /// </summary>
    protected Client()
    {
        // Forgive null here to silence warning about uninitialized field.
        _connection = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class from a
    /// Redis connection multiplexer.
    /// </summary>
    /// <param name="connection">The connection to the Redis server.</param>
    public Client(ConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class from a
    /// connectio string.
    /// </summary>
    /// <param name="connectionString">The connection string to the Redis server.</param>
    public Client(
        string connectionString
    ) : this(ConnectionMultiplexer.Connect(connectionString))
    {
    }

    /// <summary>
    /// Gets the Redis database specified in the connection string, or the
    /// default Redis database.
    /// </summary>
    protected IDatabase GetDatabase()
    {
        return _connection.GetDatabase();
    }

    /// <summary>
    /// Executes a qless command Lua script on the Redis server with the given
    /// arguments.
    /// </summary>
    /// <param name="arguments">The arguments to pass to the qless command.</param>
    public virtual async Task<RedisResult> ExecuteAsync(params RedisValue[] arguments)
    {
        return await GetDatabase().ScriptEvaluateAsync(_luaScript, values: arguments);
    }

    /// <inheritdoc />
    public async Task<bool> CancelJobAsync(string jid)
    {
        var result = await ExecuteAsync([
            "cancel",
            GetNow(),
            jid,
        ]);

        var resultJid = (string?)result
            ?? throw new InvalidOperationException("Expected jid to be non-null.");

        return resultJid == jid;
    }

    /// <inheritdoc />
    public async Task<bool> FailAsync(
        string jid,
        string workerName,
        string group,
        string message,
        string? data = null
    )
    {
        var result = await ExecuteAsync([
            "fail",
            GetNow(),
            jid,
            workerName,
            group,
            message,
            data
        ]);

        var resultJid = (string?)result
            ?? throw new InvalidOperationException("Expected jid to be non-null.");

        return resultJid == jid;
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, int>> FailedCountsAsync()
    {
        var result = await ExecuteAsync(["failed", GetNow()]);

        var failedCountsJson = (string?)result
            ?? throw new InvalidOperationException("Received unexpected null result.");

        var keyValues = JsonSerializer.Deserialize<Dictionary<string, int>>(
            failedCountsJson
        ) ?? throw new InvalidOperationException("Failed to deserialize JSON.");

        return keyValues;
    }

    /// <inheritdoc/>
    public async Task<Job?> GetJobAsync(string jid)
    {
        var result = await ExecuteAsync(["get", GetNow(), jid]);

        var jobJson = (string?)result
            ?? throw new InvalidOperationException("Received unexpected null result.");

        Console.WriteLine(jobJson);

        var job = JsonSerializer.Deserialize<Job>(jobJson)
          ?? throw new InvalidOperationException("Failed to deserialize JSON.");

        return job;
    }

    /// <inheritdoc/>
    public async Task<Job?> PopAsync(string queueName, string workerName)
    {
        var jobs = await PopMultiAsync(queueName, workerName, 1);
        return jobs.Count == 0 ? null : jobs[0];
    }

    /// <inheritdoc/>
    public async Task<List<Job>> PopMultiAsync(
        string queueName,
        string workerName,
        int count
    )
    {
        var result = await ExecuteAsync([
            "pop",
            GetNow(),
            queueName,
            workerName,
            count
        ]);
        var jobsJson = (string?)result
            ?? throw new InvalidOperationException("Expected jobs JSON to be non-null.");

        // Redis cjson can't distinguish between an empty array and an empty
        // object, so an empty object here actually represents an empty
        // array, and, ergo, no jobs retrieved.
        if (jobsJson == "{}")
        {
            return [];
        }

        Console.WriteLine($"Jobs JSON: {jobsJson}");

        var jobs = JsonSerializer.Deserialize<List<Job>>(jobsJson)
            ?? throw new InvalidOperationException("Deserialize jobs JSON failed.");

        return jobs;
    }

    /// <inheritdoc/>
    public async Task<string> PutAsync(
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
        var result = await ExecuteAsync([
            "put",
            GetNow(),
            workerName,
            queueName,
            jid ?? Guid.NewGuid().ToString("N"),
            className,
            data,
            delay,
            "depends",
            JsonSerializer.Serialize(dependencies ?? []),
            "priority",
            priority,
            "retries",
            retries,
            "tags",
            JsonSerializer.Serialize(tags ?? []),
            "throttles",
            JsonSerializer.Serialize(throttles ?? []),
        ]);

        var resultJid = (string?)result
            ?? throw new InvalidOperationException("Expected jid to be non-null.");

        return resultJid;
    }

    /// <inheritdoc/>
    public async Task<bool> RetryAsync(
        string jid,
        string queueName,
        string workerName,
        string group,
        string message,
        int delay = 0
    )
    {
        var result = await ExecuteAsync([
            "retry",
            GetNow(),
            jid,
            queueName,
            workerName,
            delay,
            group,
            message,
        ]);

        var remainingRetries = (int?)result
            ?? throw new InvalidOperationException("Expected result to be non-null.");

        return remainingRetries > 0;
    }

    /// <summary>
    /// Gets the current time in milliseconds since the Unix epoch for use as
    /// the now argument when invoking qless commands.
    /// </summary>
    /// <remarks>
    /// This method is virtual to allow for easy mocking in tests.
    /// </remarks>
    protected virtual long GetNow()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Releases all resources used by the <see cref="Client"/>.
    /// </summary>
    public virtual void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases all resources used by the <see cref="Client"/>.
    /// </summary>
    /// <param name="disposing">True if disposing, false otherwise.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _disposed = true;
            _connection.Dispose();
        }
    }
}