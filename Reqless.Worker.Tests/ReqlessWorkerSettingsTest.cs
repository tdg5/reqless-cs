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
                Maybe<ReadOnlyCollection<string>>.Some(
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
        var settings = MakeSubject(queueIdentifiers: Maybe.Some(queueIdentifiers));
        Assert.Equal(queueIdentifiers, settings.QueueIdentifiers);
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
        var settings = MakeSubject(workerCount: Maybe.Some(workerCount));
        Assert.Equal(workerCount, settings.WorkerCount);
    }

    /// <summary>
    /// Create a new instance of <see cref="ReqlessWorkerSettings"/> with the
    /// specified parameters or default values.
    /// </summary>
    /// <param name="queueIdentifiers">The queue identifiers.</param>
    /// <param name="workerCount">The number of workers.</param>
    public static ReqlessWorkerSettings MakeSubject(
        Maybe<ReadOnlyCollection<string>>? queueIdentifiers = null,
        Maybe<int>? workerCount = null
    )
    {
        Maybe<ReadOnlyCollection<string>> _queueIdentifiers = queueIdentifiers
            ?? Maybe<ReadOnlyCollection<string>>.Some(
                new List<string> { "queue1", "queue2" }.AsReadOnly()
            );
        Maybe<int> _workerCount = workerCount ?? Maybe<int>.Some(1);
        return new ReqlessWorkerSettings(
            queueIdentifiers: _queueIdentifiers.GetOrDefault(null!),
            workerCount: _workerCount.GetOrDefault(1)
        );
    }
}
