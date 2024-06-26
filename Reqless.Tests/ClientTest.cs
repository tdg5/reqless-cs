using System.Text.Json;
using Moq;
using Reqless.Models;
using StackExchange.Redis;

namespace Reqless.Tests;

/// <summary>
/// Unit tests for <see cref="Client"/>.
/// </summary>
public class ClientTest
{
    /// <summary>
    /// CancelJobAsync should call ExecuteAsync with the expected arguments.
    /// </summary>
    [Fact]
    public async void CancelJobAsync_CallsExecuteAsyncWithTheExpectedArguments()
    {
        var jid = "jid";

        var expectedArguments = new RedisValue[] {
            "cancel",
            0,
            jid,
        };
        var executeAsyncMock = new Mock<Client>();
        executeAsyncMock.Setup(
            mock => mock.ExecuteAsync(It.Is<RedisValue[]>(
                args => args.SequenceEqual(expectedArguments)
            ))
        ).Returns(
            Task.FromResult(RedisResult.Create((RedisValue)jid))
        );
        using var subject = new ConnectionlessClient(executeAsyncMock.Object);
        var cancelledSuccessfully = await subject.CancelJobAsync(jid: jid);
        executeAsyncMock.VerifyAll();
        executeAsyncMock.VerifyNoOtherCalls();
        Assert.True(cancelledSuccessfully);
    }

    /// <summary>
    /// FailAsync should call ExecuteAsync with the expected arguments.
    /// </summary>
    [Fact]
    public async void FailAsync_CallsExecuteAsyncWithTheExpectedArguments()
    {
        var data = "{}";
        var group = "group";
        var message = "message";
        var jid = "jid";
        var workerName = "workerName";

        var expectedArguments = new RedisValue[] {
            "fail",
            0,
            jid,
            workerName,
            group,
            message,
            data
        };
        var executeAsyncMock = new Mock<Client>();
        executeAsyncMock.Setup(
            mock => mock.ExecuteAsync(It.Is<RedisValue[]>(
                args => args.SequenceEqual(expectedArguments)
            ))
        ).Returns(
            Task.FromResult(RedisResult.Create((RedisValue)jid))
        );
        using var subject = new ConnectionlessClient(executeAsyncMock.Object);
        var failedSuccessfully = await subject.FailAsync(
            data: data,
            group: group,
            jid: jid,
            message: message,
            workerName: workerName
        );
        executeAsyncMock.VerifyAll();
        executeAsyncMock.VerifyNoOtherCalls();
        Assert.True(failedSuccessfully);
    }

    /// <summary>
    /// FailedCountAsync should call ExecuteAsync with the expected arguments.
    /// </summary>
    [Fact]
    public async void FailedCountAsync_CallsExecuteAsyncWithTheExpectedArguments()
    {
        var expectedArguments = new RedisValue[] {"failed", 0};
        var executeAsyncMock = new Mock<Client>();
        var expectedResult = new Dictionary<string, int>() {
            { "group", 1 },
        };
        var failedCountsJson = JsonSerializer.Serialize(expectedResult);
        executeAsyncMock.Setup(
            mock => mock.ExecuteAsync(It.Is<RedisValue[]>(
                args => args.SequenceEqual(expectedArguments)
            ))
        ).Returns(
            Task.FromResult(RedisResult.Create((RedisValue)failedCountsJson))
        );
        using var subject = new ConnectionlessClient(executeAsyncMock.Object);
        var failedCounts = await subject.FailedCountsAsync();
        executeAsyncMock.VerifyAll();
        executeAsyncMock.VerifyNoOtherCalls();
        Assert.Equivalent(expectedResult, failedCounts);
    }

    /// <summary>
    /// PutAsync should call ExecuteAsync with the expected arguments.
    /// </summary>
    [Fact]
    public async void PutAsync_CallsExecuteAsyncWithTheExpectedArguments()
    {
        var workerName = "workerName";
        var queueName = "queueName";
        var className = "className";
        var data = "{}";
        var delay = 0;
        var jid = "jid";
        var priority = 0;
        var retries = 5;
        var dependencies = new string[] { "dependencies" };
        var tags = new string[] { "tags" };
        var throttles = new string[] { "throttles" };

        var expectedArguments = new RedisValue[] {
            "put",
            0,
            workerName,
            queueName,
            jid,
            className,
            data,
            delay,
            "depends",
            JsonSerializer.Serialize(dependencies),
            "priority",
            priority,
            "retries",
            retries,
            "tags",
            JsonSerializer.Serialize(tags),
            "throttles",
            JsonSerializer.Serialize(throttles),
        };
        var executeAsyncMock = new Mock<Client>();
        executeAsyncMock.Setup(
            mock => mock.ExecuteAsync(It.Is<RedisValue[]>(
                args => args.SequenceEqual(expectedArguments)
            ))
        ).Returns(
            Task.FromResult(RedisResult.Create((RedisValue)jid))
        );
        using var subject = new ConnectionlessClient(executeAsyncMock.Object);
        var jidFromPut = await subject.PutAsync(
            workerName,
            queueName,
            className,
            data,
            delay,
            jid,
            priority,
            retries,
            dependencies,
            tags,
            throttles
        );
        executeAsyncMock.VerifyAll();
        executeAsyncMock.VerifyNoOtherCalls();
        Assert.Equal(jid, jidFromPut);
    }

    /// <summary>
    /// RetryAsync should call ExecuteAsync with the expected arguments.
    /// </summary>
    [Fact]
    public async void RetryAsync_CallsExecuteAsyncWithTheExpectedArguments()
    {
        var workerName = "workerName";
        var queueName = "queueName";
        var group = "group";
        var message = "message";
        var delay = 0;
        var jid = "jid";

        var expectedArguments = new RedisValue[] {
            "retry",
            0,
            jid,
            queueName,
            workerName,
            delay,
            group,
            message,
        };
        var executeAsyncMock = new Mock<Client>();
        executeAsyncMock.Setup(
            mock => mock.ExecuteAsync(It.Is<RedisValue[]>(
                args => args.SequenceEqual(expectedArguments)
            ))
        ).Returns(
            Task.FromResult(RedisResult.Create((RedisValue)1))
        );
        using var subject = new ConnectionlessClient(executeAsyncMock.Object);
        var retriedSuccessfully = await subject.RetryAsync(
            jid,
            queueName,
            workerName,
            group,
            message,
            delay
        );
        executeAsyncMock.VerifyAll();
        executeAsyncMock.VerifyNoOtherCalls();
        Assert.True(retriedSuccessfully);
    }

    /// <summary>
    /// A test-only version of Client that doesn't require a connection.
    /// </summary>
    protected class ConnectionlessClient : Client
    {
        private Client MockInstance { get; }

        private long Now { get; set; } = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionlessClient"/>
        /// class with a default mock instance that expects no calls.
        /// </summary>
        public ConnectionlessClient() : this(new Mock<Client>().Object)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionlessClient"/>
        /// class using the given mock instance. The mock instance is used only
        /// for mocking calls to ExecuteAsync.
        /// </summary>
        /// <param name="mockInstance">The mock instance to use to mock select
        /// method calls.</param>
        public ConnectionlessClient(Client mockInstance) : base()
        {
            MockInstance = mockInstance;
        }

        /// <inheritdoc />
        public override async Task<RedisResult> ExecuteAsync(
            params RedisValue[] arguments
        )
        {
            return await MockInstance.ExecuteAsync(arguments);
        }

        /// <inheritdoc />
        protected override long GetNow()
        {
            return Now++;
        }

        /// <inheritdoc />
        /// <remarks>
        /// This method is overridden to prevent the base class from disposing
        /// the non-existant connection.
        /// </remarks>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _disposed = true;
            }
        }
    }
}