using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetFailedJobsByGroupAsync"/>.
/// </summary>
public class GetFailedJobsByGroupTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetFailedJobsByGroupAsync"/> should throw if group
    /// name is null.
    /// </summary>
    [Fact]
    public async void ShouldThrowIfGroupNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetFailedJobsByGroupAsync(groupName: null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'groupName')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetFailedJobsByGroupAsync"/> should throw if group
    /// name is empty or only whitespace.
    /// </summary>
    [Fact]
    public async void ShouldThrowIfGroupNameIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.GetFailedJobsByGroupAsync(groupName: emptyString)
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'groupName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetFailedJobsByGroupAsync"/> should throw if
    /// server returns null.
    /// </summary>
    [Fact]
    public async void ShouldThrowIfServerReturnsNull()
    {
        var limit = 25;
        var offset = 0;
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetFailedJobsByGroupAsync(
                    groupName: ExampleGroupName,
                    limit: limit,
                    offset: offset
                ),
                expectedArguments: [
                    "jobs.failedByGroup",
                    0,
                    ExampleGroupName,
                    offset,
                    limit
                ],
                returnValue: null
            )
        );
        Assert.Equal(
            "Server returned unexpected null result.",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetFailedJobsByGroupAsync"/> should throw if
    /// JSON can't be deserialized.
    /// </summary>
    [Fact]
    public async void ShouldThrowIfJsonCannotBeDeserialized()
    {
        var limit = 25;
        var offset = 0;
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetFailedJobsByGroupAsync(
                    groupName: ExampleGroupName,
                    limit: limit,
                    offset: offset
                ),
                expectedArguments: [
                    "jobs.failedByGroup",
                    0,
                    ExampleGroupName,
                    offset,
                    limit
                ],
                returnValue: "null"
            )
        );
        Assert.Equal(
            "Failed to deserialize failed jobs query result JSON: null",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetFailedJobsByGroupAsync"/> should return
    /// JidsResult.
    /// </summary>
    [Fact]
    public async void ShouldReturnJidsResult()
    {
        var limit = 25;
        var offset = 0;
        var total = 21;
        var jids = new List<string> { "jid1", "jid2" };
        var jobsJson = JsonSerializer.Serialize(jids);
        var result = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetFailedJobsByGroupAsync(
                groupName: ExampleGroupName,
                limit: limit,
                offset: offset
            ),
            expectedArguments: [
                "jobs.failedByGroup",
                0,
                ExampleGroupName,
                offset,
                limit
            ],
            returnValue: $$"""{"total":{{total}},"jobs":{{jobsJson}}}"""
        );
        Assert.IsType<JidsResult>(result);
        Assert.Equal(total, result.Total);
        Assert.Equal(jids, result.Jids);
    }
}