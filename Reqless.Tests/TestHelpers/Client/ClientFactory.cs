using Reqless.Client;
using StackExchange.Redis;

namespace Reqless.Tests.TestHelpers.Client;

/// <summary>
/// Factory for creating <see cref="ReqlessClient"/> instances for testing.
/// </summary>
public static class ClientFactory
{
    /// <summary>
    /// Creates a new <see cref="ReqlessClient"/> instance for testing.
    /// </summary>
    /// <param name="connectionString">The connection string to use when
    /// creating the client.</param>
    /// <param name="flushDatabase">Flag indicating whether or not the database
    /// should be flushed before creating the client.</param>
    public static ReqlessClient Client(
        bool flushDatabase = true,
        string? connectionString = null
    )
    {
        string _connectionString = connectionString ?? "localhost:6379,allowAdmin=true";
        var connection = ConnectionMultiplexer.Connect(_connectionString);
        if (flushDatabase)
        {
            connection.GetServers().First().FlushDatabase();
        }
        return new ReqlessClient(connection);
    }
}