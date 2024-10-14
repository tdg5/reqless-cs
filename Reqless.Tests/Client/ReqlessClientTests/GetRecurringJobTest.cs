using Reqless.Client.Models;
using Reqless.Client;
using Reqless.Common.Utilities;
using Reqless.Tests.Common.Client.Models;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetRecurringJobAsync"/>.
/// </summary>
public class GetRecurringJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetRecurringJobAsync"/> should throw if the
    /// given job ID is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetRecurringJobAsync(invalidJid!)
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetRecurringJobAsync"/> should return null if
    /// the server returns null.
    /// </summary>
    [Fact]
    public async Task ReturnsNullIfTheServerReturnsNull()
    {
        RecurringJob? result = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetRecurringJobAsync(ExampleJid),
            expectedArguments: ["recurringJob.get", 0, ExampleJid],
            returnValue: null
        );
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetRecurringJobAsync"/> should throw if the
    /// recurring job JSON can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfResultJsonCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetRecurringJobAsync(ExampleJid),
                expectedArguments: ["recurringJob.get", 0, ExampleJid],
                returnValue: "null"
            )
        );
        Assert.Equal(
            "Failed to deserialize recurring job JSON: null",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetRecurringJobAsync"/> should return the
    /// recurring job when it exists.
    /// </summary>
    [Fact]
    public async Task ReturnsTheRecurringJob()
    {
        var recurringJobJson = RecurringJobFactory.RecurringJobJson(
            jid: Maybe.Some(ExampleJid)
        );
        RecurringJob? result = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetRecurringJobAsync(ExampleJid),
            expectedArguments: ["recurringJob.get", 0, ExampleJid],
            returnValue: recurringJobJson
        );
        Assert.IsType<RecurringJob>(result);
        Assert.Equal(ExampleJid, result.Jid);
    }
}
