using System.Collections.ObjectModel;
using Reqless.Common.Utilities;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Worker.Tests;

/// <summary>
/// Unit tests for the <see cref="ReqlessWorkerSettings"/> class.
/// </summary>
public class ReqlessWorkerSettingsTest
{
    /// <summary>
    /// <see cref="ReqlessWorkerSettings"/> constructor should throw if the
    /// connectionString argument is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenConnectionStringIsNullOrEmptyOrWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidConnectionString) => MakeSubject(
                connectionString: Maybe.Some(invalidConnectionString!)
            ),
            "connectionString"
        );
    }

    /// <summary>
    /// <see cref="ReqlessWorkerSettings"/> constructor should set the
    /// <see cref="ReqlessWorkerSettings.ConnectionString"/> property.
    /// </summary>
    [Fact]
    public void Constructor_SetsConnectionStringProperty()
    {
        var connectionString = "localhost:6379";
        var subject = MakeSubject(connectionString: Maybe.Some(connectionString));
        Assert.Equal(connectionString, subject.ConnectionString);
    }

    /// <summary>
    /// <see cref="ReqlessWorkerSettings"/> constructor should throw if the
    /// queueIdentifiers argument is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenQueueIdentifiersIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeSubject(
                queueIdentifiers: Maybe<ReadOnlyCollection<string>>.Some(null!)
            )
        );
        Assert.Equal(
            $"Value cannot be null. (Parameter 'queueIdentifiers')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessWorkerSettings"/> constructor should throw if the
    /// queueIdentifiers argument is empty.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenQueueIdentifiersIsEmpty()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmpty<string>(
            (invalidQueueIdentifiers) => MakeSubject(
                queueIdentifiers: Maybe<ReadOnlyCollection<string>>.Some(
                    invalidQueueIdentifiers?.AsReadOnly()!
                )
            ),
            "queueIdentifiers"
        );
    }

    /// <summary>
    /// <see cref="ReqlessWorkerSettings"/> constructor should set the
    /// <see cref="ReqlessWorkerSettings.QueueIdentifiers"/> property.
    /// </summary>
    [Fact]
    public void Constructor_SetsQueueIdentifiersProperty()
    {
        var queueIdentifiers = new List<string> { "queue1", "queue2" }.AsReadOnly();
        var subject = MakeSubject(queueIdentifiers: Maybe.Some(queueIdentifiers));
        Assert.Equal(queueIdentifiers, subject.QueueIdentifiers);
    }

    /// <summary>
    /// <see cref="ReqlessWorkerSettings"/> constructor should throw if the
    /// workerCount argument is not positive.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenWorkerCountIsNotPositive()
    {
        Scenario.ThrowsWhenArgumentIsNotPositive(
            (invalidWorkerCount) => MakeSubject(
                workerCount: Maybe<int>.Some(invalidWorkerCount)
            ),
            "workerCount"
        );
    }

    /// <summary>
    /// <see cref="ReqlessWorkerSettings"/> constructor should set the
    /// <see cref="ReqlessWorkerSettings.WorkerCount"/> property.
    /// </summary>
    [Fact]
    public void Constructor_SetsWorkerCountProperty()
    {
        var workerCount = 2;
        var subject = MakeSubject(workerCount: Maybe.Some(workerCount));
        Assert.Equal(workerCount, subject.WorkerCount);
    }

    /// <summary>
    /// Create a new instance of <see cref="ReqlessWorkerSettings"/> with the
    /// specified parameters or default values.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="queueIdentifiers">The queue identifiers.</param>
    /// <param name="workerCount">The number of workers.</param>
    public static ReqlessWorkerSettings MakeSubject(
        Maybe<string>? connectionString = null,
        Maybe<ReadOnlyCollection<string>>? queueIdentifiers = null,
        Maybe<int>? workerCount = null
    )
    {
        Maybe<string> _connectionString = connectionString
            ?? Maybe<string>.Some("localhost:6379");
        Maybe<ReadOnlyCollection<string>> _queueIdentifiers = queueIdentifiers
            ?? Maybe<ReadOnlyCollection<string>>.Some(
                new List<string> { "queue1", "queue2" }.AsReadOnly()
            );
        Maybe<int> _workerCount = workerCount ?? Maybe<int>.Some(1);
        return new ReqlessWorkerSettings(
            connectionString: _connectionString.GetOrDefault(null!),
            queueIdentifiers: _queueIdentifiers.GetOrDefault(null!),
            workerCount: _workerCount.GetOrDefault(1)
        );
    }
}
