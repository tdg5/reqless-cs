using Reqless.Models;
using StackExchange.Redis;

namespace Reqless.Tests;

/// <summary>
/// Integration tests for <see cref="Client"/>.
/// </summary>
public class ClientIntegrationTest
{

    private static readonly ConnectionMultiplexer _connection =
        ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");

    public ClientIntegrationTest()
    {
        _connection.GetServers().First().FlushDatabase();
    }

    /// <summary>
    /// PutAsync should be able to put and receive jid result.
    /// </summary>
    [Fact]
    async public void PutAsync_CanPutAndReceiveJid()
    {
        var className = "className";
        var data = "{}";
        var dependencyJid = "dependencyJid";
        var dependencies = new string[] { dependencyJid };
        var priority = 0;
        var queueName = "queueName";
        var retries = 5;
        var tags = new string[] { "tags" };
        var throttles = new string[] { "throttles" };
        var workerName = "workerName";

        using var client = new Client(_connection);
        var dependencyPutJid = await client.PutAsync(
            className: className,
            data: data,
            jid: dependencyJid,
            priority: priority,
            queueName: queueName,
            retries: retries,
            tags: tags,
            throttles: throttles,
            workerName: workerName
        );

        Assert.Equal(dependencyJid, dependencyPutJid);

        var jid = await client.PutAsync(
            className: className,
            data: data,
            dependencies: dependencies,
            priority: priority,
            queueName: queueName,
            retries: retries,
            tags: tags,
            throttles: throttles,
            workerName: workerName
        );

        Job? dependency = await client.GetJobAsync(dependencyJid);
        Assert.NotNull(dependency);
        Assert.Equivalent(new string[] { jid }, dependency.Dependents);
        Assert.Equal("waiting", dependency.State);

        Job? subject = await client.GetJobAsync(jid);
        Assert.NotNull(subject);
        Assert.Equal(className, subject.ClassName);
        Assert.Equal(data, subject.Data);
        Assert.Equivalent(dependencies, subject.Dependencies);
        Assert.Equal(priority, subject.Priority);
        Assert.Equal(queueName, subject.QueueName);
        Assert.Equal(retries, subject.Remaining);
        Assert.Equal(retries, subject.Retries);
        Assert.Equivalent(tags, subject.Tags);
        Assert.Equivalent(throttles, subject.Throttles);
        Assert.False(subject.Tracked);
        Assert.Null(subject.SpawnedFromJid);
        Assert.Null(subject.Failure);
        Assert.Equal("depends", subject.State);

        await client.CancelJobAsync(jid);
        await client.CancelJobAsync(dependencyJid);
    }
}