using System.Text.RegularExpressions;
using Moq;
using Reqless.Client;
using StackExchange.Redis;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Base class for testing <see cref="ReqlessClient"/>.
/// </summary>
public partial class BaseReqlessClientTest
{
    /// <summary>
    /// An example class name.
    /// </summary>
    protected static readonly string ExampleClassName = "example-class-name";

    /// <summary>
    /// Example data.
    /// </summary>
    protected static readonly string ExampleData = "{}";

    /// <summary>
    /// An example failure group.
    /// </summary>
    protected static readonly string ExampleGroup = "example-group";

    /// <summary>
    /// An example job ID.
    /// </summary>
    protected static readonly string ExampleJid = "example-jid";

    /// <summary>
    /// Another example job ID.
    /// </summary>
    protected static readonly string ExampleJidOther = "example-jid-other";

    /// <summary>
    /// An example message.
    /// </summary>
    protected static readonly string ExampleMessage = "example-message";

    /// <summary>
    /// An example queue name.
    /// </summary>
    protected static readonly string ExampleQueueName = "example-queue-name";

    /// <summary>
    /// An example tag.
    /// </summary>
    protected static readonly string ExampleTag = "example-tag";

    /// <summary>
    /// Another example tag.
    /// </summary>
    protected static readonly string ExampleTagOther = "example-tag-other";

    /// <summary>
    /// Example throttle.
    /// </summary>
    protected static readonly string ExampleThrottle = "example-throttle";

    /// <summary>
    /// An example worker name.
    /// </summary>
    protected static readonly string ExampleWorkerName = "example-worker-name";

    /// <summary>
    /// A Regex for matching a UUID-based job ID.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex("^[0-9a-f]{32}$")]
    protected static partial Regex JidRegex();

    /// <summary>
    /// Helper method for creating a ReqlessClient with a mocked RedisExecutor
    /// and running an action with the client.
    /// </summary>
    /// <param name="action">A function that takes a ReqlessClient and tests
    /// <param name="expectedArguments">The arguments that are expected to be
    /// passed to ExecuteAsync.</param>
    /// <param name="returnValue">Singular RedisValue to return from
    /// ExecuteAsync.</param> <param name="returnValues">Array of RedisValue to
    /// return from ExecuteAsync.</param>
    /// it.</param>
    protected static async Task WithClientWithExecutorMockForExpectedArguments(
        Func<ReqlessClient, Task> action,
        RedisValue[]? expectedArguments = null,
        RedisValue? returnValue = null,
        RedisValue[]? returnValues = null
    )
    {
        if (returnValue is not null && returnValues is not null)
        {
            throw new ArgumentException(
                $"{nameof(returnValue)} and {nameof(returnValues)} cannot both be set."
            );

        }

        Mock<RedisExecutor> executorMock;
        if (expectedArguments is null)
        {
            executorMock = new Mock<RedisExecutor>();
        }
        else
        {
            RedisResult result = returnValues is not null ?
                RedisResult.Create(returnValues) :
                RedisResult.Create(returnValue ?? RedisValue.Null);

            executorMock = ExecutorMockForExpectedArguments(
                expectedArguments,
                Task.FromResult(result)
            );
        }
        using var subject = new PredictableNowReqlessClient(executorMock.Object);
        await action(subject);
        executorMock.VerifyAll();
        executorMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Helper method to create a Mock{RedisExecutor} that verifies ExecuteAsync
    /// is called with the expected arguments and returns the given return
    /// value.
    /// </summary>
    /// <param name="expectedArguments">The arguments that are expected to be
    /// passed to ExecuteAsync.</param>
    /// <param name="returnValue">The value to return from ExecuteAsync.</param>
    protected static Mock<RedisExecutor> ExecutorMockForExpectedArguments(
        RedisValue[] expectedArguments,
        Task<RedisResult> returnValue
    )
    {
        var executorMock = new Mock<RedisExecutor>();
        executorMock.Setup(
            mock => mock.ExecuteAsync(It.Is<RedisValue[]>(
                args => args.SequenceEqual(expectedArguments)
            ))
        ).Returns(returnValue);
        return executorMock;
    }
}
