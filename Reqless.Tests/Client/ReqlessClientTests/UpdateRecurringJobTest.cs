using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.UpdateRecurringJobAsync"/>.
/// </summary>
public class UpdateRecurringJobTest : BaseReqlessClientTest
{
    private const int ExampleIntervalSeconds = 3600;

    private const int ExampleMaximumBacklog = 10;

    private const int ExamplePriority = 1;

    private const int ExampleRetries = 5;

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should throw if
    /// jid is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UpdateRecurringJobAsync(
                    jid: invalidJid!
                )
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should throw if
    /// given className is empty or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfClassNameIsEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsEmptyOrWhitespaceAsync(
            (invalidClassName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UpdateRecurringJobAsync(
                    className: invalidClassName!,
                    jid: ExampleJid
                )
            ),
            "className"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should throw if
    /// given data is empty or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfDataIsEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsEmptyOrWhitespaceAsync(
            (invalidData) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UpdateRecurringJobAsync(
                    data: invalidData!,
                    jid: ExampleJid
                )
            ),
            "data"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should throw if
    /// given intervalSeconds is less than 1.
    /// </summary>
    [Fact]
    public async Task ThrowsIfIntervalSecondsIsNotPositive()
    {
        await Scenario.ThrowsWhenArgumentIsNotPositiveAsync(
            (invalidIntervalSeconds) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UpdateRecurringJobAsync(
                    intervalSeconds: invalidIntervalSeconds,
                    jid: ExampleJid
                )
            ),
            "intervalSeconds"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should throw if
    /// given queueName is empty or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfQueueNameIsEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UpdateRecurringJobAsync(
                    jid: ExampleJid,
                    queueName: invalidQueueName!
                )
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should throw if
    /// given throttles is contains values that are null, empty, or only
    /// whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfThrottlesContainsAnyValueThatIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespaceAsync(
            (invalidThrottle) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UpdateRecurringJobAsync(
                    jid: ExampleJid,
                    throttles: [invalidThrottle!]
                )
            ),
            "throttles"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should include the
    /// class name in the arguments when appropriate.
    /// </summary>
    [Fact]
    public async Task ClassNameArgumentIsIncludedWhenAppropriate()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                jid: ExampleJid,
                queueName: ExampleQueueName
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "queue",
                ExampleQueueName
            ]
        );
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                className: ExampleClassName,
                jid: ExampleJid,
                queueName: ExampleQueueName
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "klass",
                ExampleClassName,
                "queue",
                ExampleQueueName
            ]
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should include the
    /// data in the arguments when appropriate.
    /// </summary>
    [Fact]
    public async Task DataArgumentIsIncludedWhenAppropriate()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                jid: ExampleJid,
                queueName: ExampleQueueName
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "queue",
                ExampleQueueName
            ]
        );
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                data: ExampleData,
                jid: ExampleJid,
                queueName: ExampleQueueName
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "data",
                ExampleData,
                "queue",
                ExampleQueueName
            ]
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should include the
    /// intervalSeconds in the arguments when appropriate.
    /// </summary>
    [Fact]
    public async Task IntervalSecondsArgumentIsIncludedWhenAppropriate()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                jid: ExampleJid,
                queueName: ExampleQueueName
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "queue",
                ExampleQueueName
            ]
        );
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                intervalSeconds: ExampleIntervalSeconds,
                jid: ExampleJid,
                queueName: ExampleQueueName
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "interval",
                ExampleIntervalSeconds,
                "queue",
                ExampleQueueName
            ]
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should include the
    /// maximum backlog in the arguments when appropriate.
    /// </summary>
    [Fact]
    public async Task MaximumBacklogArgumentIsIncludedWhenAppropriate()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                className: ExampleClassName,
                jid: ExampleJid
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "klass",
                ExampleClassName,
            ]
        );
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                className: ExampleClassName,
                jid: ExampleJid,
                maximumBacklog: ExampleMaximumBacklog
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "klass",
                ExampleClassName,
                "backlog",
                ExampleMaximumBacklog,
            ]
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should include the
    /// priority in the arguments when appropriate.
    /// </summary>
    [Fact]
    public async Task PriorityArgumentIsIncludedWhenAppropriate()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                className: ExampleClassName,
                jid: ExampleJid
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "klass",
                ExampleClassName,
            ]
        );
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                className: ExampleClassName,
                jid: ExampleJid,
                priority: ExamplePriority
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "klass",
                ExampleClassName,
                "priority",
                ExamplePriority,
            ]
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should include the
    /// queue name in the arguments when appropriate.
    /// </summary>
    [Fact]
    public async Task QueueNameArgumentIsIncludedWhenAppropriate()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                className: ExampleClassName,
                jid: ExampleJid
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "klass",
                ExampleClassName,
            ]
        );
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                className: ExampleClassName,
                jid: ExampleJid,
                queueName: ExampleQueueName
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "klass",
                ExampleClassName,
                "queue",
                ExampleQueueName,
            ]
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should include the
    /// retries in the arguments when appropriate.
    /// </summary>
    [Fact]
    public async Task RetriesArgumentIsIncludedWhenAppropriate()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                className: ExampleClassName,
                jid: ExampleJid
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "klass",
                ExampleClassName,
            ]
        );
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                className: ExampleClassName,
                jid: ExampleJid,
                retries: ExampleRetries
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "klass",
                ExampleClassName,
                "retries",
                ExampleRetries,
            ]
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should include the
    /// throttles in the arguments when appropriate.
    /// </summary>
    [Fact]
    public async Task ThrottlesArgumentIsIncludedWhenAppropriate()
    {
        string[] throttles = [ExampleThrottleName];
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                className: ExampleClassName,
                jid: ExampleJid
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "klass",
                ExampleClassName,
            ]
        );
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(
                className: ExampleClassName,
                jid: ExampleJid,
                throttles: throttles
            ),
            expectedArguments: [
                "recurringJob.update",
                0,
                ExampleJid,
                "klass",
                ExampleClassName,
                "throttles",
                JsonSerializer.Serialize(throttles),
            ]
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> doesn't engage the
    /// executor if all optional arguments are null.
    /// </summary>
    [Fact]
    public async Task NoRequestIsMadeWhenAllArgumentsAreNull()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UpdateRecurringJobAsync(jid: ExampleJid)
        );
    }
}
