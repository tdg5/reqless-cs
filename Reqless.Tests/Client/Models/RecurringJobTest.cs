using Reqless.Client.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.Models;

/// <summary>
/// Tests for the <see cref="RecurringJob"/> class.
/// </summary>
public class RecurringJobTest
{
    /// <summary>
    /// The constructor should throw an exception if the given className
    /// argument is null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ClassName_ThrowsWhenNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidClassName) => MakeRecurringJob(className: invalidClassName),
            "className"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given count argument
    /// is negative.
    /// </summary>
    [Fact]
    public void Constructor_Count_ThrowsWhenNegative()
    {
        Scenario.ThrowsWhenArgumentIsNegative(
            (invalidCount) => MakeRecurringJob(count: Maybe<int>.Some(invalidCount)),
            "count"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given data argument is
    /// null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Data_ThrowsWhenNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidData) => MakeRecurringJob(data: invalidData),
            "data"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given intervalSeconds argument
    /// is not positive.
    /// </summary>
    [Fact]
    public void Constructor_IntervalSeconds_ThrowsWhenNegative()
    {
        Scenario.ThrowsWhenArgumentIsNotPositive(
            (invalidIntervalSeconds) => MakeRecurringJob(
                intervalSeconds: Maybe<int>.Some(invalidIntervalSeconds)
            ),
            "intervalSeconds"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given jid argument is
    /// null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Jid_ThrowsWhenNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidJid) => MakeRecurringJob(jid: invalidJid),
            "jid"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given maximumBacklog
    /// argument is negative.
    /// </summary>
    [Fact]
    public void Constructor_MaximumBacklog_ThrowsWhenNegative()
    {
        Scenario.ThrowsWhenArgumentIsNegative(
            (invalidMaximumBacklog) => MakeRecurringJob(
                maximumBacklog: Maybe<int>.Some(invalidMaximumBacklog)
            ),
            "maximumBacklog"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given priority argument
    /// is negative.
    /// </summary>
    [Fact]
    public void Constructor_Priority_ThrowsWhenNegative()
    {
        Scenario.ThrowsWhenArgumentIsNegative(
            (invalidPriority) => MakeRecurringJob(
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
        var job = MakeRecurringJob(queueName: null);
        Assert.Null(job.QueueName);
    }

    /// <summary>
    /// The constructor should throw an exception if the given queueName
    /// argument is empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_QueueName_ThrowsWhenEmptyOrWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsEmptyOrWhitespace(
            (invalidQueueName) => MakeRecurringJob(queueName: invalidQueueName),
            "queueName"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given retries argument
    /// is negative.
    /// </summary>
    [Fact]
    public void Constructor_Retries_ThrowsWhenNegative()
    {
        Scenario.ThrowsWhenArgumentIsNegative(
            (invalidRetries) => MakeRecurringJob(
                retries: Maybe<int>.Some(invalidRetries)
            ),
            "retries"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given state argument is
    /// null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void Constructor_State_ThrowsWhenNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidState) => MakeRecurringJob(state: invalidState),
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
        Scenario.ThrowsWhenArgumentIsNull(
            () => MakeRecurringJob(tags: Maybe<string[]?>.Some(null)),
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
        Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespace(
            (invalidTag) => MakeRecurringJob(
                tags: Maybe<string[]?>.Some([invalidTag!])
            ),
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
        Scenario.ThrowsWhenArgumentIsNull(
            () => MakeRecurringJob(throttles: Maybe<string[]?>.Some(null)),
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
        Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespace(
            (invalidThrottle) => MakeRecurringJob(
                throttles: Maybe<string[]?>.Some([invalidThrottle!])
            ),
            "throttles"
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
    /// <param name="count">The count of the job.</param>
    /// <param name="data">The data of the job.</param>
    /// <param name="intervalSeconds">The interval in seconds of the
    /// job.</param>
    /// <param name="jid">The job ID of the job.</param>
    /// <param name="maximumBacklog">The maximum backlog of the job.</param>
    /// <param name="priority">The priority of the job.</param>
    /// <param name="queueName">The queue name of the job.</param>
    /// <param name="retries">The total number of retries of the job.</param>
    /// <param name="state">The state of the job.</param>
    /// <param name="tags">The tags applied to the job.</param>
    /// <param name="throttles">The throttles applied to the job.</param>
    public static RecurringJob MakeRecurringJob(
        string? className = "className",
        Maybe<int>? count = null,
        string? data = "{}",
        Maybe<int>? intervalSeconds = null,
        string? jid = "jid",
        Maybe<int>? maximumBacklog = null,
        Maybe<int>? priority = null,
        string? queueName = "queueName",
        Maybe<int>? retries = null,
        string? state = "state",
        Maybe<string[]?>? tags = null,
        Maybe<string[]?>? throttles = null
    )
    {
        int _count = (count ?? Maybe<int>.None).GetOrDefault(0);
        int _intervalSeconds = (intervalSeconds ?? Maybe<int>.None).GetOrDefault(60);
        int _maximumBacklog = (maximumBacklog ?? Maybe<int>.None).GetOrDefault(10);
        int _priority = (priority ?? Maybe<int>.None).GetOrDefault(0);
        int _retries = (retries ?? Maybe<int>.None).GetOrDefault(5);
        string[]? _tags = (tags ?? Maybe<string[]?>.None).GetOrDefault([]);
        string[]? _throttles = (throttles ?? Maybe<string[]?>.None).GetOrDefault([]);
        return new RecurringJob(
            className: className!,
            count: _count,
            data: data!,
            intervalSeconds: _intervalSeconds,
            jid: jid!,
            maximumBacklog: _maximumBacklog,
            priority: _priority,
            queueName: queueName!,
            retries: _retries,
            state: state!,
            tags: _tags!,
            throttles: _throttles!
        );
    }
}