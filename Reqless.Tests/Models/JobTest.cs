using Reqless.Models;
using Reqless.Models.JobEvents;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Models;

/// <summary>
/// Tests for the <see cref="Job"/> class.
/// </summary>
public class JobTest
{
    static readonly string[] EmptyStrings = ["", " ", "\t"];

    /// <summary>
    /// The constructor should throw an exception if the given className
    /// argument is null.
    /// </summary>
    [Fact]
    public void Constructor_ClassName_ThrowsWhenNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeJob(className: null)
        );
        Assert.Equal("className", exception.ParamName);
        Assert.Equal(
            "Value cannot be null. (Parameter 'className')",
            exception.Message
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given className
    /// argument is empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ClassName_ThrowsWhenEmptyOrWhitespace()
    {
        foreach (var invalidClassName in EmptyStrings)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MakeJob(className: invalidClassName)
            );
            Assert.Equal("className", exception.ParamName);
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'className')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given data argument is
    /// null.
    /// </summary>
    [Fact]
    public void Constructor_Data_ThrowsWhenNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeJob(data: null)
        );
        Assert.Equal("data", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'data')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an exception if the given data is empty or
    /// composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Data_ThrowsWhenEmptyOrWhitespace()
    {
        foreach (var invalidData in EmptyStrings)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MakeJob(data: invalidData)
            );
            Assert.Equal("data", exception.ParamName);
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'data')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given dependencies
    /// argument is null.
    /// </summary>
    [Fact]
    public void Constructor_Dependencies_ThrowsWhenNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeJob(dependencies: Maybe<string[]?>.Some(null))
        );
        Assert.Equal("dependencies", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'dependencies')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an exception if the given dependents
    /// argument is null.
    /// </summary>
    [Fact]
    public void Constructor_Dependents_ThrowsWhenNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeJob(dependents: Maybe<string[]?>.Some(null))
        );
        Assert.Equal("dependents", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'dependents')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an exception if the given expires argument
    /// is negative or zero.
    /// </summary>
    [Fact]
    public void Constructor_Expires_ThrowsWhenNotPostive()
    {
        foreach (var nonPositiveValue in new long[] { -100, -1, 0 })
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => MakeJob(expires: Maybe<long>.Some(nonPositiveValue))
            );
            Assert.Equal("expires", exception.ParamName);
            Assert.Equal(
                $"""
                expires must be a positive whole number. (Parameter 'expires')
                Actual value was {nonPositiveValue}.
                """,
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given history
    /// argument is null.
    /// </summary>
    [Fact]
    public void Constructor_History_ThrowsWhenNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeJob(history: Maybe<JobEvent[]?>.Some(null))
        );
        Assert.Equal("history", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'history')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an exception if the given jid argument is
    /// null.
    /// </summary>
    [Fact]
    public void Constructor_Jid_ThrowsWhenNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeJob(jid: null)
        );
        Assert.Equal("jid", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'jid')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an exception if the given jid argument is
    /// empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Jid_ThrowsWhenEmptyOrWhitespace()
    {
        foreach (var invalidJid in EmptyStrings)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MakeJob(jid: invalidJid)
            );
            Assert.Equal("jid", exception.ParamName);
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'jid')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given priority argument
    /// is negative.
    /// </summary>
    [Fact]
    public void Constructor_Priority_ThrowsWhenNegative()
    {
        foreach (var negativeValue in new int[] { -100, -1 })
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => MakeJob(priority: Maybe<int>.Some(negativeValue))
            );
            Assert.Equal("priority", exception.ParamName);
            Assert.Equal(
                $"""
                priority must be a non-negative whole number. (Parameter 'priority')
                Actual value was {negativeValue}.
                """,
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given queueName
    /// argument is null.
    /// </summary>
    [Fact]
    public void Constructor_QueueName_ThrowsWhenNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeJob(queueName: null)
        );
        Assert.Equal("queueName", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'queueName')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an exception if the given queueName
    /// argument is empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_QueueName_ThrowsWhenEmptyOrWhitespace()
    {
        foreach (var invalidQueueName in EmptyStrings)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MakeJob(queueName: invalidQueueName)
            );
            Assert.Equal("queueName", exception.ParamName);
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'queueName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given remaining argument
    /// is negative.
    /// </summary>
    [Fact]
    public void Constructor_Remaining_ThrowsWhenNegative()
    {
        foreach (var negativeValue in new int[] { -100, -1 })
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => MakeJob(remaining: Maybe<int>.Some(negativeValue))
            );
            Assert.Equal("remaining", exception.ParamName);
            Assert.Equal(
                $"""
                remaining must be a non-negative whole number. (Parameter 'remaining')
                Actual value was {negativeValue}.
                """,
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given remaining argument
    /// is greater than the given retries argument.
    /// </summary>
    [Fact]
    public void Constructor_Remaining_ThrowsWhenRemainingGreaterThanRetries()
    {
        var remaining = 10;
        var retries = 5;
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => MakeJob(
                remaining: Maybe<int>.Some(remaining),
                retries: Maybe<int>.Some(retries)
            )
        );
        Assert.Equal("remaining", exception.ParamName);
        Assert.Equal(
            $"""
            remaining must be less than or equal to retries ({retries}). (Parameter 'remaining')
            Actual value was {remaining}.
            """,
            exception.Message
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given retries argument
    /// is negative.
    /// </summary>
    [Fact]
    public void Constructor_Retries_ThrowsWhenNegative()
    {
        foreach (var negativeValue in new int[] { -100, -1 })
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => MakeJob(retries: Maybe<int>.Some(negativeValue))
            );
            Assert.Equal("retries", exception.ParamName);
            Assert.Equal(
                $"""
                retries must be a non-negative whole number. (Parameter 'retries')
                Actual value was {negativeValue}.
                """,
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given spawnedFromJid
    /// argument is empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_SpawnedFromJid_ThrowsWhenEmptyOrWhitespace()
    {
        foreach (var invalidSpawnedFromJid in EmptyStrings)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MakeJob(spawnedFromJid: invalidSpawnedFromJid)
            );
            Assert.Equal("spawnedFromJid", exception.ParamName);
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'spawnedFromJid')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given state argument is
    /// null.
    /// </summary>
    [Fact]
    public void Constructor_State_ThrowsWhenNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeJob(state: null)
        );
        Assert.Equal("state", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'state')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an exception if the given state argument is
    /// empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_State_ThrowsWhenEmptyOrWhitespace()
    {
        foreach (var invalidState in EmptyStrings)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MakeJob(state: invalidState)
            );
            Assert.Equal("state", exception.ParamName);
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'state')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given tags argument is
    /// null.
    /// </summary>
    [Fact]
    public void Constructor_Tags_ThrowsWhenNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeJob(tags: Maybe<string[]?>.Some(null))
        );
        Assert.Equal("tags", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'tags')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an exception if the given throttles
    /// argument is null.
    /// </summary>
    [Fact]
    public void Constructor_Throttles_ThrowsWhenNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeJob(dependents: Maybe<string[]?>.Some(null))
        );
        Assert.Equal("dependents", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'dependents')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an exception if the given workerName
    /// argument is null.
    /// </summary>
    [Fact]
    public void Constructor_WorkerName_ThrowsWhenNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeJob(workerName: null)
        );
        Assert.Equal("workerName", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'workerName')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an exception if the given workerName
    /// argument is empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_WorkerName_ThrowsWhenEmptyOrWhitespace()
    {
        foreach (var invalidWorkerName in EmptyStrings)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MakeJob(workerName: invalidWorkerName)
            );
            Assert.Equal("workerName", exception.ParamName);
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'workerName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// Helper method to create a job instance from sane defaults or given
    /// arguments.
    /// </summary>
    /// <remarks>
    /// The usage of Maybe types is to allow explicit arguments to be
    /// distinguished from default arguments. Passing null or Maybe.None can
    /// be used to opt for a default argument. Passing Maybe.Some is used to
    /// provide an explicit value.
    /// </remarks>
    /// <param name="className">The class name of the job.</param>
    /// <param name="data">The data of the job.</param>
    /// <param name="dependencies">The dependencies of the job.</param>
    /// <param name="dependents">The dependents of the job.</param>
    /// <param name="expires">The expiry time of the job.</param>
    /// <param name="failure">The failure data for the job.</param>
    /// <param name="history">The history of the job.</param>
    /// <param name="jid">The job ID of the job.</param>
    /// <param name="priority">The priority of the job.</param>
    /// <param name="queueName">The queue name of the job.</param>
    /// <param name="remaining">The remaining retries of the job.</param>
    /// <param name="retries">The total number of retries of the job.</param>
    /// <param name="spawnedFromJid">The job ID of the job that spawned the job.</param>
    /// <param name="state">The state of the job.</param>
    /// <param name="tags">The tags applied to the job.</param>
    /// <param name="throttles">The throttles applied to the job.</param>
    /// <param name="tracked">Whether the job is tracked.</param>
    /// <param name="workerName">The name of the worker that is currently working on the job, if any.</param>
    public static Job MakeJob(
        string? className = "className",
        string? data = "{}",
        Maybe<string[]?>? dependencies = null,
        Maybe<string[]?>? dependents = null,
        Maybe<long>? expires = null,
        Maybe<JobFailure?>? failure = null,
        Maybe<JobEvent[]?>? history = null,
        string? jid = "jid",
        Maybe<int>? priority = null,
        string? queueName = "queueName",
        Maybe<int>? remaining = null,
        Maybe<int>? retries = null,
        string? spawnedFromJid = "spawnedFromJid",
        string? state = "state",
        Maybe<string[]?>? tags = null,
        Maybe<string[]?>? throttles = null,
        bool tracked = false,
        string? workerName = "workerName"
    )
    {
        string[]? _dependencies = (dependencies ?? Maybe<string[]?>.None).GetOrDefault([]);
        string[]? _dependents = (dependents ?? Maybe<string[]?>.None).GetOrDefault([]);
        long _expires = (expires ?? Maybe<long>.None)
            .GetOrDefault(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 60000);
        JobFailure? _failure = (failure ?? Maybe<JobFailure?>.Some(null)).GetOrDefault(null);
        JobEvent[]? _history = (history ?? Maybe<JobEvent[]?>.Some([])).GetOrDefault([]);
        int _priority = (priority ?? Maybe<int>.None).GetOrDefault(0);
        int _remaining = (remaining ?? Maybe<int>.None).GetOrDefault(5);
        int _retries = (retries ?? Maybe<int>.None).GetOrDefault(5);
        string[]? _tags = (tags ?? Maybe<string[]?>.None).GetOrDefault([]);
        string[]? _throttles = (throttles ?? Maybe<string[]?>.None).GetOrDefault([]);
        return new Job(
            className: className!,
            data: data!,
            dependencies: _dependencies!,
            dependents: _dependents!,
            expires: _expires,
            failure: _failure,
            history: _history!,
            jid: jid!,
            priority: _priority,
            queueName: queueName!,
            remaining: _remaining,
            retries: _retries,
            spawnedFromJid: spawnedFromJid,
            state: state!,
            tags: _tags!,
            throttles: _throttles!,
            tracked: tracked,
            workerName: workerName!
        );
    }
}