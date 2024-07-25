using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.RemoveDependencyFromJobAsync"/>.
/// </summary>
public class RemoveDependencyFromJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.RemoveDependencyFromJobAsync"/> should throw if
    /// jid is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveDependencyFromJobAsync(
                    dependsOnJid: ExampleJid,
                    jid: null!
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'jid')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveDependencyFromJobAsync"/> should throw if
    /// jid is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.RemoveDependencyFromJobAsync(
                        jid: emptyString,
                        dependsOnJid: ExampleJid
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
    /// <see cref="ReqlessClient.RemoveDependencyFromJobAsync"/> should throw if
    /// dependsOnJid is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfDependsOnJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveDependencyFromJobAsync(
                    dependsOnJid: null!,
                    jid: ExampleJid
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'dependsOnJid')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveDependencyFromJobAsync"/> should throw if
    /// jid is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfDependsOnJidIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.RemoveDependencyFromJobAsync(
                        jid: ExampleJid,
                        dependsOnJid: emptyString
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'dependsOnJid')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveDependencyFromJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        bool result = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.RemoveDependencyFromJobAsync(
                dependsOnJid: ExampleJidOther,
                jid: ExampleJid
            ),
            expectedArguments: [
                "job.removeDependency",
                0,
                ExampleJid,
                ExampleJidOther,
            ],
            returnValue: true
        );
        Assert.True(result);
    }
}