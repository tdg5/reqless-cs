using Reqless.Client;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// A test-only version of Client that overrides GetNow to return a
/// predictable value.
/// </summary>
public class PredictableNowReqlessClient : ReqlessClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PredictableNowReqlessClient"/>
    /// class from a connection string. The given connection string is used
    /// to instantiate a new <see cref="RedisExecutor"/>. When this
    /// constructor is used, the client will take responsibility for
    /// disposing the executor.
    /// </summary>
    /// <param name="connectionString">The connection string to the Redis
    /// server.</param>
    public PredictableNowReqlessClient(string connectionString) : base(connectionString)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PredictableNowReqlessClient"/>
    /// class using the given executor instance.
    /// </summary>
    /// <param name="executor">The executor to use for executing
    /// commands.</param>
    public PredictableNowReqlessClient(IRedisExecutor executor) : base(executor)
    {
    }

    private long NowValue { get; set; } = 0;

    /// <inheritdoc />
    protected override long Now() => NowValue++;
}
