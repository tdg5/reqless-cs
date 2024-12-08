using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.FailJobAsync"/>.
/// </summary>
public class FailJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.FailJobAsync"/> should throw if data is
    /// empty or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfDataIsEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsEmptyOrWhitespaceAsync(
            (invalidData) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.FailJobAsync(
                    data: invalidData!,
                    groupName: ExampleGroupName,
                    jid: ExampleJid,
                    message: ExampleMessage,
                    workerName: ExampleWorkerName)),
            "data");
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailJobAsync"/> should throw if jid is
    /// null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.FailJobAsync(
                    data: ExampleData!,
                    groupName: ExampleGroupName,
                    jid: invalidJid!,
                    message: ExampleMessage,
                    workerName: ExampleWorkerName)),
            "jid");
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailJobAsync"/> should throw if group name
    /// is null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfGroupNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidGroupName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.FailJobAsync(
                    data: ExampleData!,
                    groupName: invalidGroupName!,
                    jid: ExampleJid,
                    message: ExampleMessage,
                    workerName: ExampleWorkerName)),
            "groupName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailJobAsync"/> should throw if message is
    /// null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfMessageIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidMessage) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.FailJobAsync(
                    data: ExampleData!,
                    groupName: ExampleGroupName,
                    jid: ExampleJid,
                    message: invalidMessage!,
                    workerName: ExampleWorkerName)),
            "message");
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailJobAsync"/> should throw if worker name
    /// is null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfWorkerNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.FailJobAsync(
                    data: ExampleData!,
                    groupName: ExampleGroupName,
                    jid: ExampleJid,
                    message: ExampleMessage,
                    workerName: invalidWorkerName!)),
            "workerName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailJobAsync"/> should call Executor with
    /// the expected arguments when data is given.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArgumentsWhenGivenData()
    {
        bool failedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.FailJobAsync(
                data: ExampleData,
                groupName: ExampleGroupName,
                jid: ExampleJid,
                message: ExampleMessage,
                workerName: ExampleWorkerName),
            expectedArguments: [
                "job.fail",
                0,
                ExampleJid,
                ExampleWorkerName,
                ExampleGroupName,
                ExampleMessage,
                ExampleData
            ],
            returnValue: ExampleJid);
        Assert.True(failedSuccessfully);
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailJobAsync"/> should call Executor with
    /// the expected arguments when data is not given.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArgumentsWhenNotGivenData()
    {
        bool failedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.FailJobAsync(
                groupName: ExampleGroupName,
                jid: ExampleJid,
                message: ExampleMessage,
                workerName: ExampleWorkerName),
            expectedArguments: [
                "job.fail",
                0,
                ExampleJid,
                ExampleWorkerName,
                ExampleGroupName,
                ExampleMessage
            ],
            returnValue: ExampleJid);
        Assert.True(failedSuccessfully);
    }
}
