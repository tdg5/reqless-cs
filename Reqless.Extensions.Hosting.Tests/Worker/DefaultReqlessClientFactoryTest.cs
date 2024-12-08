using Reqless.Client;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Tests for <see cref="DefaultReqlessClientFactory"/>.
/// </summary>
public class DefaultReqlessClientFactoryTest
{
    /// <summary>
    /// <see cref="DefaultReqlessClientFactory(IWorkerSettings)"/> throws when
    /// the given settings argument is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenSettingsIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new DefaultReqlessClientFactory(null!), "settings");
    }

    /// <summary>
    /// <see cref="DefaultReqlessClientFactory.Create()"/> creates a new
    /// reqless client with the expected settings.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void Create_CreatesReqlessClientWithExpectedSettings()
    {
        var expectedConnectionString = "localhost:6379";
        var settings = new WorkerSettings(
            connectionString: expectedConnectionString,
            queueIdentifiers: ["some-queue"],
            workerCount: 1);
        var subject = new DefaultReqlessClientFactory(settings);
        var client = subject.Create();
        Assert.IsAssignableFrom<IReqlessClient>(client);

        // There's no way to verify the connection string is correct so settle
        // for no exception being raised when creating the client.
    }
}
