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
    /// jid is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () =>
            WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddEventToJobHistoryAsync(
                    jid: null!,
                    what: ExampleMessage
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'jid')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddEventToJobHistoryAsync"/> should throw if
    /// jid is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.AddEventToJobHistoryAsync(
                        jid: emptyString,
                        what: ExampleMessage
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'jid')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddEventToJobHistoryAsync"/> should throw if
    /// what is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfWhatIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddEventToJobHistoryAsync(
                    jid: ExampleJid,
                    what: null!
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'what')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddEventToJobHistoryAsync"/> should throw if
    /// what is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfWhatIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.AddEventToJobHistoryAsync(
                        jid: ExampleJid,
                        what: emptyString
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'what')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddEventToJobHistoryAsync"/> should throw if
    /// data is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfDataIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.AddEventToJobHistoryAsync(
                        data: emptyString,
                        jid: ExampleJid,
                        what: ExampleMessage
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'data')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddEventToJobHistoryAsync"/> should call
    /// Executor with the expected arguments when not given data.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArgumentsWithoutData()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                var result = await subject.AddEventToJobHistoryAsync(
                    data: ExampleData,
                    jid: ExampleJid,
                    what: ExampleMessage
                );
                Assert.True(result);
            },
            expectedArguments: [
                "job.log",
                0,
                ExampleJid,
                ExampleMessage,
                ExampleData,
            ],
            returnValue: true
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddEventToJobHistoryAsync"/> should call
    /// Executor with the expected arguments when given data.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArgumentsWithData()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                var result = await subject.AddEventToJobHistoryAsync(
                    jid: ExampleJid,
                    what: ExampleMessage
                );
                Assert.True(result);
            },
            expectedArguments: [
                "job.log",
                0,
                ExampleJid,
                ExampleMessage,
            ],
            returnValue: true
        );
    }
}
