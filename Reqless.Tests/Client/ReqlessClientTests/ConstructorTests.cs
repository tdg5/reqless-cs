using Reqless.Client;
using StackExchange.Redis;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient"/> constructors.
/// </summary>
public class ConstructorsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient(string)"/> taking a connection string
    /// instantiates a new <see cref="RedisExecutor"/>.
    /// </summary>
    [Fact]
    public void ConnectionString_TriesToConnectToRedisViaExecutor()
    {
        var connectionString = "intentionally-bad-hostname:9999";
        var exeception = Assert.Throws<RedisConnectionException>(
            () => new PredictableNowReqlessClient(connectionString)
        );
        Assert.Contains("It was not possible to connect", exeception.Message);
    }
}