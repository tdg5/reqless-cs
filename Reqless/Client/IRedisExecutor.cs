using StackExchange.Redis;

namespace Reqless.Client;

/// <summary>
/// Interface for a client that can execute commands against a Redis-backed Reqless
/// server.
/// </summary>
public interface IRedisExecutor
{
    /// <summary>
    /// Executes the given arguments as a Reqless command.
    /// </summary>
    /// <param name="arguments">The arguments of a Reqless command.</param>
    Task<RedisResult> ExecuteAsync(params RedisValue[] arguments);
}
