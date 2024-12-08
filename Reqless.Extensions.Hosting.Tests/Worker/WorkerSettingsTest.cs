using Moq;
using Reqless.Common.Utilities;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Unit tests for the <see cref="WorkerSettings"/> class.
/// </summary>
public class WorkerSettingsTest
{
    /// <summary>
    /// <see cref="WorkerSettings"/> constructor should throw if the
    /// connectionString argument is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenConnectionStringIsNullOrEmptyOrWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidConnectionString) => MakeSubject(
                connectionString: Maybe.Some(invalidConnectionString!)),
            "connectionString");
    }

    /// <summary>
    /// <see cref="WorkerSettings"/> constructor should set the
    /// <see cref="WorkerSettings.ConnectionString"/> property.
    /// </summary>
    [Fact]
    public void Constructor_SetsConnectionStringProperty()
    {
        var connectionString = "localhost:6379";
        var subject = MakeSubject(connectionString: Maybe.Some(connectionString));
        Assert.Equal(connectionString, subject.ConnectionString);
    }

    /// <summary>
    /// <see cref="WorkerSettings"/> constructor should throw if the
    /// queueIdentifiers argument is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenQueueIdentifiersIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeSubject(
                queueIdentifiers: Maybe<List<string>>.Some(null!)));
        Assert.Equal(
            $"Value cannot be null. (Parameter 'queueIdentifiers')",
            exception.Message);
    }

    /// <summary>
    /// <see cref="WorkerSettings"/> constructor should throw if the
    /// queueIdentifiers argument is empty.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenQueueIdentifiersIsEmpty()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmpty<string>(
            (invalidQueueIdentifiers) => MakeSubject(
                queueIdentifiers: Maybe<List<string>>.Some(
                    invalidQueueIdentifiers)),
            "queueIdentifiers");
    }

    /// <summary>
    /// <see cref="WorkerSettings"/> constructor should set the
    /// <see cref="WorkerSettings.QueueIdentifiers"/> property.
    /// </summary>
    [Fact]
    public void Constructor_SetsQueueIdentifiersProperty()
    {
        var queueIdentifiers = new List<string> { "queue1", "queue2" };
        var subject = MakeSubject(queueIdentifiers: Maybe.Some(queueIdentifiers));
        Assert.Equal(queueIdentifiers, subject.QueueIdentifiers);
    }

    /// <summary>
    /// <see cref="WorkerSettings"/> constructor should throw if the
    /// workerCount argument is not positive.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenWorkerCountIsNotPositive()
    {
        Scenario.ThrowsWhenArgumentIsNotPositive(
            (invalidWorkerCount) => MakeSubject(
                workerCount: Maybe<int>.Some(invalidWorkerCount)),
            "workerCount");
    }

    /// <summary>
    /// <see cref="WorkerSettings"/> constructor should set the
    /// <see cref="WorkerSettings.WorkerCount"/> property.
    /// </summary>
    [Fact]
    public void Constructor_SetsWorkerCountProperty()
    {
        var workerCount = 2;
        var subject = MakeSubject(workerCount: Maybe.Some(workerCount));
        Assert.Equal(workerCount, subject.WorkerCount);
    }

    /// <summary>
    /// <see cref="WorkerSettings"/> constructor should set the <see
    /// cref="WorkerSettings.WorkerServiceRegistrar"/> property to the
    /// default value if the workerServiceRegistrar argument is null.
    /// </summary>
    [Fact]
    public void Constructor_WorkerServiceRegistrarDefaultsToExpectedType()
    {
        var subject = MakeSubject();
        Assert.IsType<DefaultWorkerServiceRegistrar>(subject.WorkerServiceRegistrar);
    }

    /// <summary>
    /// <see cref="WorkerSettings"/> constructor should set the <see
    /// cref="WorkerSettings.WorkerServiceRegistrar"/> property to the
    /// provided value if the workerServiceRegistrar argument is not null.
    /// </summary>
    [Fact]
    public void Constructor_WorkerServiceRegistrarCanBeProvided()
    {
        var workerServiceRegistrar = new Mock<IWorkerServiceRegistrar>().Object;
        var subject = MakeSubject(
            workerServiceRegistrar: Maybe<IWorkerServiceRegistrar?>.Some(
                workerServiceRegistrar));
        Assert.Same(workerServiceRegistrar, subject.WorkerServiceRegistrar);
    }

    /// <summary>
    /// <see cref="WorkerSettings"/> constructor should set the <see
    /// cref="WorkerSettings.ConnectionString"/> property to the default value
    /// if the connectionString argument is not provided.
    /// </summary>
    [Fact]
    public void Constructor_DefaultsConnectionStringToExpectedValue()
    {
        WorkerSettings subject = new(["queue"], 5);
        Assert.Equal("localhost:6379", subject.ConnectionString);
    }

    /// <summary>
    /// Create a new instance of <see cref="WorkerSettings"/> with the
    /// specified parameters or default values.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="queueIdentifiers">The queue identifiers.</param>
    /// <param name="workerCount">The number of workers.</param>
    /// <param name="workerServiceRegistrar">The worker service registrar.</param>
    /// <returns>A new instance of <see cref="WorkerSettings"/>.</returns>
    private static WorkerSettings MakeSubject(
        Maybe<string>? connectionString = null,
        Maybe<List<string>>? queueIdentifiers = null,
        Maybe<int>? workerCount = null,
        Maybe<IWorkerServiceRegistrar?>? workerServiceRegistrar = null)
    {
        Maybe<string> connectionStringOrDefault = connectionString
            ?? Maybe<string>.Some("localhost:6379");
        Maybe<List<string>> queueIdentifiersOrDefault = queueIdentifiers
            ?? Maybe<List<string>>.Some(["queue1", "queue2"]);
        Maybe<int> workerCountOrDefault = workerCount ?? Maybe<int>.Some(1);
        Maybe<IWorkerServiceRegistrar?> workerServiceRegistrarOrDefault =
            workerServiceRegistrar ?? Maybe<IWorkerServiceRegistrar?>.None;
        return new WorkerSettings(
            connectionString: connectionStringOrDefault.GetOrDefault(null!),
            queueIdentifiers: queueIdentifiersOrDefault.GetOrDefault(null!),
            workerCount: workerCountOrDefault.GetOrDefault(1),
            workerServiceRegistrar: workerServiceRegistrarOrDefault.GetOrDefault(null));
    }
}
