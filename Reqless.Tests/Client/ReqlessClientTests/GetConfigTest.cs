using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetConfigAsync"/>.
/// </summary>
public class GetConfigTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetConfigAsync"/> should throw if configName is
    /// null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfConfigNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidConfigName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetConfigAsync(invalidConfigName!)
            ),
            "configName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetConfigAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithExpectedArguments()
    {
        var configName = "config-name";
        var value = "config-value";
        var returnValue = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetConfigAsync(configName),
            expectedArguments: [
                "config.get",
                0,
                configName,
            ],
            returnValue: value
        );
        Assert.Equal(value, returnValue);
    }
}
