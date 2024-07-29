using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.AddEventToJobHistoryAsync"/>.
/// </summary>
public class AddEventToJobHistoryTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.AddEventToJobHistoryAsync"/> should throw if
    /// jid is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddEventToJobHistoryAsync(
                    jid: invalidJid!,
                    what: ExampleMessage
                )
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddEventToJobHistoryAsync"/> should throw if
    /// what is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfWhatIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidWhat) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddEventToJobHistoryAsync(
                    jid: ExampleJid,
                    what: invalidWhat!
                )
            ),
            "what"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddEventToJobHistoryAsync"/> should throw if
    /// data is empty or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfDataIsEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsEmptyOrWhitespaceAsync(
            (invalidData) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddEventToJobHistoryAsync(
                    data: invalidData,
                    jid: ExampleJid,
                    what: ExampleMessage
                )
            ),
            "data"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddEventToJobHistoryAsync"/> should call
    /// Executor with the expected arguments when not given data.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArgumentsWithoutData()
    {
        var result = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.AddEventToJobHistoryAsync(
                data: ExampleData,
                jid: ExampleJid,
                what: ExampleMessage
            ),
            expectedArguments: [
                "job.log",
                0,
                ExampleJid,
                ExampleMessage,
                ExampleData,
            ],
            returnValue: true
        );
        Assert.True(result);
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddEventToJobHistoryAsync"/> should call
    /// Executor with the expected arguments when given data.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArgumentsWithData()
    {
        var result = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.AddEventToJobHistoryAsync(
                jid: ExampleJid,
                what: ExampleMessage
            ),
            expectedArguments: [
                "job.log",
                0,
                ExampleJid,
                ExampleMessage,
            ],
            returnValue: true
        );
        Assert.True(result);
    }
}
