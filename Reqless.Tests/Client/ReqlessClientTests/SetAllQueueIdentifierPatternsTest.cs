using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.SetAllQueueIdentifierPatternsAsync"/>.
/// </summary>
public class SetAllQueueIdentifierPatternsTest : BaseReqlessClientTest
{

    /// <summary>
    /// <see cref="ReqlessClient.SetAllQueueIdentifierPatternsAsync"/> should
    /// throw if identifier patterns argument is null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfIdentifierPatternsIsNull()
    {
        await Scenario.ThrowsArgumentNullExceptionAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetAllQueueIdentifierPatternsAsync(null!)
            ),
            "identifierPatterns"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetAllQueueIdentifierPatternsAsync"/> should
    /// throw if any identifier is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfIdentifierIsNullEmptyOrOnlyWhitespace()
    {
        // Dictionary throws if the key is null, so only check empty string and
        // whitespace.
        await Scenario.ThrowsWhenParameterIsEmptyOrWhitespaceAsync(
            (invalidIdentifier) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetAllQueueIdentifierPatternsAsync(
                    new Dictionary<string, IEnumerable<string>>()
                    {
                        [invalidIdentifier!] = ["something"],
                    }
                )
            ),
            "identifierPatterns.key"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetAllQueueIdentifierPatternsAsync"/> should
    /// throw if patterns is null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfPatternsIsNull()
    {
        await Scenario.ThrowsArgumentNullExceptionAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetAllQueueIdentifierPatternsAsync(
                    new Dictionary<string, IEnumerable<string>>()
                    {
                        ["identifier"] = null!,
                    }
                )
            ),
            "identifierPatterns.value"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetAllQueueIdentifierPatternsAsync"/> should
    /// throw if any pattern is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfIAnyPatternsListIsNullEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterItemIsNullOrEmptyOrWhitespaceAsync(
            (invalidPattern) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetAllQueueIdentifierPatternsAsync(
                    new Dictionary<string, IEnumerable<string>>()
                    {
                        ["identifier"] = [invalidPattern!],
                    }
                )
            ),
            "identifierPatterns.value"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetAllQueueIdentifierPatternsAsync"/> should
    /// not include identifiers with empty patterns when calling the executor.
    /// </summary>
    [Fact]
    public async Task DoesNotCallExecutorWithEmptyPatterns()
    {
        string identifier = "identifier";
        string otherIdentifier = "other-identifier";
        string[] patterns = ["*"];
        string[] otherPatterns = [];
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.SetAllQueueIdentifierPatternsAsync(
                new Dictionary<string, IEnumerable<string>>()
                {
                    [identifier] = patterns,
                    [otherIdentifier] = otherPatterns,
                }
            ),
            expectedArguments: [
                "queueIdentifierPatterns.setAll",
                0,
                identifier,
                JsonSerializer.Serialize(patterns),
            ]
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetAllQueueIdentifierPatternsAsync"/> should
    /// call the executor with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithExpectedArguments()
    {
        string identifier = "identifier";
        string otherIdentifier = "other-identifier";
        string[] patterns = ["*"];
        string[] otherPatterns = ["something"];
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.SetAllQueueIdentifierPatternsAsync(
                new Dictionary<string, IEnumerable<string>>()
                {
                    [identifier] = patterns,
                    [otherIdentifier] = otherPatterns,
                }
            ),
            expectedArguments: [
                "queueIdentifierPatterns.setAll",
                0,
                identifier,
                JsonSerializer.Serialize(patterns),
                otherIdentifier,
                JsonSerializer.Serialize(otherPatterns),
            ]
        );
    }
}