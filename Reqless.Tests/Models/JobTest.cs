using Reqless.Models;
using Reqless.Models.JobEvents;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Models;

/// <summary>
/// Tests for the <see cref="Job"/> class.
/// </summary>
public class JobTest
{
    /// <summary>
    /// The constructor should throw an exception if the given className
    /// argument is null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ClassName_ThrowsWhenNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespace(
            (invalidClassName) => MakeJob(className: invalidClassName),
            "className"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given data argument is
    /// null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Data_ThrowsWhenNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespace(
            (invalidData) => MakeJob(data: invalidData),
            "data"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given dependencies
    /// argument is null.
    /// </summary>
    [Fact]
    public void Constructor_Dependencies_ThrowsWhenNull()
    {
        Scenario.ThrowsArgumentNullException(
            () => MakeJob(dependencies: Maybe<string[]?>.Some(null)),
            "dependencies"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given dependencies
    /// include values that are null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Dependencies_ThrowsWhenAnyValueIsNullEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenParameterItemIsNullOrEmptyOrWhitespace(
            (invalidDependency) => MakeJob(
                dependencies: Maybe<string[]?>.Some([invalidDependency!])
            ),
            "dependencies"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given dependents
    /// argument is null.
    /// </summary>
    [Fact]
    public void Constructor_Dependents_ThrowsWhenNull()
    {
        Scenario.ThrowsArgumentNullException(
            () => MakeJob(dependents: Maybe<string[]?>.Some(null)),
            "dependents"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given dependencies
    /// include values that are null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Dependents_ThrowsWhenAnyValueIsNullEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenParameterItemIsNullOrEmptyOrWhitespace(
            (invalidDependent) => MakeJob(
                dependents: Maybe<string[]?>.Some([invalidDependent!])
            ),
            "dependents"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given expires argument
    /// is negative or zero.
    /// </summary>
    [Fact]
    public void Constructor_Expires_ThrowsWhenNotPostive()
    {
        Scenario.ThrowsWhenParameterIsNotPositive(
            (long invalidExpires) => MakeJob(
                expires: Maybe<long>.Some(invalidExpires)
            ),
            "expires"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given history
    /// argument is null.
    /// </summary>
    [Fact]
    public void Constructor_History_ThrowsWhenNull()
    {
        Scenario.ThrowsArgumentNullException(
            () => MakeJob(history: Maybe<JobEvent[]?>.Some(null)),
            "history"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given history
    /// includes any null values.
    /// </summary>
    [Fact]
    public void Constructor_History_ThrowsWhenAnyValueIsNull()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => MakeJob(history: Maybe<JobEvent[]?>.Some([null!]))
        );
        Assert.Equal("history", exception.ParamName);
        Assert.Equal("Value cannot include null. (Parameter 'history')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an exception if the given jid argument is
    /// null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Jid_ThrowsWhenNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespace(
            (invalidJid) => MakeJob(jid: invalidJid),
            "jid"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given priority argument
    /// is negative.
    /// </summary>
    [Fact]
    public void Constructor_Priority_ThrowsWhenNegative()
    {
        Scenario.ThrowsWhenParameterIsNegative(
            (invalidPriority) => MakeJob(
                priority: Maybe<int>.Some(invalidPriority)
            ),
            "priority"
        );
    }

    /// <summary>
    /// The constructor should allow queueName to be null.
    /// </summary>
    [Fact]
    public void Constructor_QueueName_CanBeNull()
    {
        var job = MakeJob(queueName: null);
        Assert.Null(job.QueueName);
    }

    /// <summary>
    /// The constructor should throw an exception if the given queueName
    /// argument is empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_QueueName_ThrowsWhenEmptyOrWhitespace()
    {
        Scenario.ThrowsWhenParameterIsEmptyOrWhitespace(
            (invalidQueueName) => MakeJob(queueName: invalidQueueName),
            "queueName"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given remaining argument
    /// is less than -1.
    /// </summary>
    [Fact]
    public void Constructor_Remaining_ThrowsWhenLessThanMinusOne()
    {
        foreach (var invalidValue in new int[] { -100, -2 })
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => MakeJob(remaining: Maybe<int>.Some(invalidValue))
            );
            Assert.Equal("remaining", exception.ParamName);
            Assert.Equal(
                $"""
                Value must be a whole number greater than or equal to -1. (Parameter 'remaining')
                Actual value was {invalidValue}.
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
            Value must be less than or equal to retries ({retries}). (Parameter 'remaining')
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
        Scenario.ThrowsWhenParameterIsNegative(
            (invalidRetries) => MakeJob(retries: Maybe<int>.Some(invalidRetries)),
            "retries"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given spawnedFromJid
    /// argument is empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_SpawnedFromJid_ThrowsWhenEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenParameterIsEmptyOrWhitespace(
            (invalidSpawnedFromJid) => MakeJob(spawnedFromJid: invalidSpawnedFromJid),
            "spawnedFromJid"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given state argument is
    /// null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_State_ThrowsWhenNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespace(
            (invalidState) => MakeJob(state: invalidState),
            "state"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given tags argument is
    /// null.
    /// </summary>
    [Fact]
    public void Constructor_Tags_ThrowsWhenNull()
    {
        Scenario.ThrowsArgumentNullException(
            () => MakeJob(tags: Maybe<string[]?>.Some(null)),
            "tags"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given tags include
    /// values that are null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Tags_ThrowsWhenAnyValueIsNullEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenParameterItemIsNullOrEmptyOrWhitespace(
            (invalidTag) => MakeJob(tags: Maybe<string[]?>.Some([invalidTag!])),
            "tags"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given throttles
    /// argument is null.
    /// </summary>
    [Fact]
    public void Constructor_Throttles_ThrowsWhenNull()
    {
        Scenario.ThrowsArgumentNullException(
            () => MakeJob(throttles: Maybe<string[]?>.Some(null)),
            "throttles"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given throttles include
    /// values that are null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Throttles_ThrowsWhenAnyValueIsNullEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenParameterItemIsNullOrEmptyOrWhitespace(
            (invalidThrottle) => MakeJob(
                throttles: Maybe<string[]?>.Some([invalidThrottle!])
            ),
            "throttles"
        );
    }

    /// <summary>
    /// The constructor should allow workerName to be null.
    /// </summary>
    [Fact]
    public void Constructor_WorkerName_AllowsNull()
    {
        var subject = MakeJob(workerName: null);
        Assert.Null(subject.WorkerName);
    }

    /// <summary>
    /// The constructor should throw an exception if the given workerName
    /// argument is empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_WorkerName_ThrowsWhenEmptyOrWhitespace()
    {
        Scenario.ThrowsWhenParameterIsEmptyOrWhitespace(
            (invalidWorkerName) => MakeJob(workerName: invalidWorkerName),
            "workerName"
        );
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