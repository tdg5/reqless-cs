using Moq;
using Reqless.Client;
using StackExchange.Redis;

namespace Reqless.Tests.Client;

/// <summary>
/// Tests for <see cref="RedisExecutor"/>.
/// </summary>
public class RedisExecutorTest
{
    /// <summary>
    /// A version of IConnectionMultiplexer that implements IDisposable, for
    /// mocking.
    /// </summary>
    public interface IDisposableConnectionMultiplexer
        : IConnectionMultiplexer, IDisposable
    {
    }

    /// <summary>
    /// The constructor form that takes a connection string should try to connect
    /// to the Redis server.
    /// </summary>
    [Fact]
    public void Constructor_ConnectionString_TriesToConnectToRedis()
    {
        var connectionString = "intentionally-bad-hostname:9999";
        var exeception = Assert.Throws<RedisConnectionException>(
            () => new RedisExecutor(connectionString));
        Assert.Contains("It was not possible to connect", exeception.Message);
    }

    /// <summary>
    /// The constructor form that takes a connection multiplexer should use the
    /// connection to execute commands.
    /// </summary>
    [Fact]
    public void Constructor_ConnectionMultiplexer_DoesNotDisposeConnectionLater()
    {
        var connectionMock = new Mock<IDisposableConnectionMultiplexer>();
        using (var subject = new RevealingRedisExecutor(connectionMock.Object))
        {
            Assert.Equal(connectionMock.Object, subject.GetConnection);
            Assert.False(subject.GetIsResponsibleForConnection);
        }

        // Verify that the connection was not disposed.
        connectionMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// ExecuteAsync makes the expected call to the connection.
    /// </summary>
    /// <returns>A Task denoting the completion of the test.</returns>
    [Fact]
    public async Task ExecuteAsync_MakesTheExpectedRedisCall()
    {
        var arguments = new RedisValue[] { "arg1", "arg2" };
        var databaseMock = new Mock<IDatabase>();
        var expectedResult = "{}";
        databaseMock
            .Setup(
                mock => mock.ScriptEvaluateAsync(
                    RevealingRedisExecutor.GetLuaScript,
                    default,
                    arguments,
                    CommandFlags.None))
            .Returns(Task.FromResult(RedisResult.Create((RedisValue)expectedResult)));

        var connectionMock = new Mock<IDisposableConnectionMultiplexer>();
        connectionMock
            .Setup(mock => mock.GetDatabase(-1, null))
            .Returns(databaseMock.Object);

        using (var subject = new RevealingRedisExecutor(connectionMock.Object))
        {
            var result = await subject.ExecuteAsync(arguments);
            Assert.NotNull(result);
            Assert.Equal(expectedResult, (string?)result);
        }

        databaseMock.VerifyAll();
        databaseMock.VerifyNoOtherCalls();
        connectionMock.VerifyAll();
        connectionMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// The Lua script should be readable and contain expected text.
    /// </summary>
    [Fact]
    public void LuaScriptHasExpectedContent()
    {
        Assert.Contains("ReqlessAPI", RevealingRedisExecutor.GetLuaScript);
    }

    /// <summary>
    /// Test specific subclass of <see cref="RedisExecutor"/> that exposes some
    /// protected members for testing.
    /// </summary>
    public class RevealingRedisExecutor : RedisExecutor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevealingRedisExecutor"/>
        /// class using the given connection multiplexer.
        /// </summary>
        /// <param name="connection">The connection to the Redis server.</param>
        public RevealingRedisExecutor(IConnectionMultiplexer connection)
            : base(connection)
        {
        }

        /// <summary>
        /// Gets the Lua script that implements the full set of reqless commands.
        /// </summary>
        public static string GetLuaScript => LuaScript;

        /// <summary>
        /// Gets the connection to the Redis server.
        /// </summary>
        public IConnectionMultiplexer GetConnection => Connection;

        /// <summary>
        /// Gets a value indicating whether the client is responsible for disposing
        /// the connection.
        /// </summary>
        public bool GetIsResponsibleForConnection => IsResponsibleForConnection;
    }
}
