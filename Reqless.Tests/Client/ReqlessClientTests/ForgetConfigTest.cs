using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.ForgetConfigAsync"/>.
/// </summary>
public class ForgetConfigTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.ForgetConfigAsync"/> should throw if configName is
    /// null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfConfigNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidConfigName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.ForgetConfigAsync(invalidConfigName!)),
            "configName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.ForgetConfigAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithExpectedArguments()
    {
        var configName = "config-name";
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.ForgetConfigAsync(configName),
            expectedArguments: ["config.unset", 0, configName]);
    }
}
