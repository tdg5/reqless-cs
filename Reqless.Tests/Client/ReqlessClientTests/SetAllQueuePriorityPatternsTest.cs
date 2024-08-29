using System.Text.Json;
using Reqless.Client;
using Reqless.Client.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.SetAllQueuePriorityPatternsAsync"/>.
/// </summary>
public class SetAllQueuePriorityPatternsTest : BaseReqlessClientTest
{

    /// <summary>
    /// <see cref="ReqlessClient.SetAllQueuePriorityPatternsAsync"/> should
    /// throw if patterns argument is null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfPriorityPatternsIsNull()
    {
        await Scenario.ThrowsWhenArgumentIsNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetAllQueuePriorityPatternsAsync(null!)
            ),
            "priorityPatterns"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetAllQueuePriorityPatternsAsync"/> should
    /// throw if any pattern is empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfIAnyPatternsListIsEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespaceAsync(
            (invalidPattern) => WithClientWithExecutorMockForExpectedArguments(
                async subject =>
                {
                    QueuePriorityPattern queuePriorityPattern = new(pattern: []);
                    queuePriorityPattern.Pattern.Add(invalidPattern!);
                    await subject.SetAllQueuePriorityPatternsAsync(
                        [queuePriorityPattern]
                    );
                }
            ),
            "priorityPatterns[].Pattern"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetAllQueuePriorityPatternsAsync"/> should
    /// not include priorities with empty patterns when calling the executor.
    /// </summary>
    [Fact]
    public async Task DoesNotCallExecutorWithEmptyPatterns()
    {
        var withPatterns = new QueuePriorityPattern(pattern: ["*"]);
        var withoutPatterns = new QueuePriorityPattern(pattern: []);
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.SetAllQueuePriorityPatternsAsync(
                [withPatterns, withoutPatterns]
            ),
            expectedArguments: [
                "queuePriorityPatterns.setAll",
                0,
                JsonSerializer.Serialize(withPatterns),
            ]
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetAllQueuePriorityPatternsAsync"/> should
    /// call the executor with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithExpectedArguments()
    {
        var withPatterns = new QueuePriorityPattern(pattern: ["*"]);
        var alsoWithPatterns = new QueuePriorityPattern(pattern: ["yay", "fun"]);
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.SetAllQueuePriorityPatternsAsync(
                [withPatterns, alsoWithPatterns]
            ),
            expectedArguments: [
                "queuePriorityPatterns.setAll",
                0,
                JsonSerializer.Serialize(withPatterns),
                JsonSerializer.Serialize(alsoWithPatterns),
            ]
        );
    }
}
