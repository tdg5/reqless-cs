using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.AddDependencyToJobAsync"/>.
/// </summary>
public class AddDependencyToJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.AddDependencyToJobAsync"/> should throw if jid
    /// is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddDependencyToJobAsync(
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
    /// <see cref="ReqlessClient.AddDependencyToJobAsync"/> should throw if jid
    /// is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.AddDependencyToJobAsync(
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
    /// <see cref="ReqlessClient.AddDependencyToJobAsync"/> should throw if
    /// dependsOnJid is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfDependsOnJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.AddDependencyToJobAsync(
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
    /// <see cref="ReqlessClient.AddDependencyToJobAsync"/> should throw if
    /// dependsOnJid is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfDependsOnJidIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                        subject => subject.AddDependencyToJobAsync(
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
    /// <see cref="ReqlessClient.AddDependencyToJobAsync"/> should call
    /// Executor with the expected
    /// arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        var otherJid = "otherJid";
        var result = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.AddDependencyToJobAsync(
                    dependsOnJid: otherJid,
                    jid: ExampleJid
                ),
            expectedArguments: [
                "job.addDependency",
                0,
                ExampleJid,
                otherJid
            ],
            returnValue: true
        );
        Assert.True(result);
    }
}