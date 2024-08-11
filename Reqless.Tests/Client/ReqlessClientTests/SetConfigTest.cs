using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.SetConfigAsync"/>.
/// </summary>
public class SetConfigTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.SetConfigAsync"/> should throw if configName is
    /// null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfConfigNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidConfigName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetConfigAsync(
                    configName: invalidConfigName!,
                    value: "some value"
                )
            ),
            "configName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetConfigAsync"/> should throw if value is
    /// null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfValueIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidValue) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetConfigAsync(
                    configName: "config-name",
                    value: invalidValue!
                )
            ),
            "value"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetConfigAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithExpectedArguments()
    {
        var configName = "config-name";
        var value = "config-value";
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.SetConfigAsync(
                configName: configName,
                value: value
            ),
            expectedArguments: [
                "config.set",
                0,
                configName,
                value,
            ]
        );
    }
}
