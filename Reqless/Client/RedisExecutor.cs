using StackExchange.Redis;

namespace Reqless.Client;

/// <summary>
/// A client for executing commands against a Reqless-compatible Redis server.
/// </summary>
public class RedisExecutor : IRedisExecutor, IDisposable
{
    /// <summary>
    /// The Lua script that implements the full set of reqless commands.
    /// </summary>
    protected static readonly string LuaScript =
        ResourceHelper.ReadTextResource("LuaScripts/reqless.lua");

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisExecutor"/> class from a
    /// Redis connection multiplexer. When this constructor is used, the client
    /// will not be responsible for disposing the connection.
    /// </summary>
    /// <param name="connection">The connection to the Redis server.</param>
    public RedisExecutor(IConnectionMultiplexer connection)
        : this(connection, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisExecutor"/> class from a
    /// connection string. When this constructor is used, the client will take
    /// responsibility for disposing the connection.
    /// </summary>
    /// <param name="connectionString">The connection string to the Redis server.</param>
    public RedisExecutor(string connectionString)
        : this(ConnectionMultiplexer.Connect(connectionString), true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisExecutor"/> class. This
    /// constructor is for testing purposes only.
    /// </summary>
    protected RedisExecutor()
    {
        // Forgive null here to silence warning about uninitialized field.
        Connection = null!;
        IsResponsibleForConnection = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisExecutor"/> class from a
    /// Redis connection multiplexer. When this constructor is used, the client
    /// will not be responsible for disposing the connection.
    /// </summary>
    /// <param name="connection">The connection to the Redis server.</param>
    /// <param name="responsibleForConnection">True if the client is responsible
    /// for disposing the connection, false otherwise.</param>
    protected RedisExecutor(
        IConnectionMultiplexer connection, bool responsibleForConnection)
    {
        Connection = connection;
        IsResponsibleForConnection = responsibleForConnection;
    }

    /// <summary>
    /// Gets the connection to the Redis server, concretely, <see
    /// cref="ConnectionMultiplexer"/>.
    /// </summary>
    protected IConnectionMultiplexer Connection { get; }

    /// <summary>
    /// Gets a value indicating whether the instance has been disposed or not.
    /// True if the object has been disposed, false otherwise.
    /// </summary>
    protected bool IsDisposed { get; private set; } = false;

    /// <summary>
    /// Gets a value indicating whether the client is responsible for disposing
    /// the connection or not. True if the client is responsible for disposing
    /// the connection, false otherwise.
    /// </summary>
    protected bool IsResponsibleForConnection { get; }

    /// <summary>
    /// Executes a qless command Lua script on the Redis server with the given
    /// arguments.
    /// </summary>
    /// <param name="arguments">The arguments to pass to the qless command.</param>
    /// <returns>A task containing the result of the command.</returns>
    public virtual async Task<RedisResult> ExecuteAsync(params RedisValue[] arguments)
    {
        RedisResult result = await Connection.GetDatabase().ScriptEvaluateAsync(
            LuaScript, values: arguments);
        return result;
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
            if (!IsDisposed && disposing)
            {
                IsDisposed = true;
                if (IsResponsibleForConnection && Connection is IDisposable connection)
                {
                    connection.Dispose();
                }
            }
        }
    }
}
