using Reqless.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Models;

/// <summary>
/// Tests for the <see cref="RecurringJob"/> class.
/// </summary>
public class RecurringJobTest
{
    /// <summary>
    /// The constructor should throw an exception if the given className
    /// argument is null.
    /// </summary>
    [Fact]
    public void Constructor_ClassName_ThrowsWhenNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeRecurringJob(className: null)
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
        foreach (var invalidClassName in TestConstants.EmptyStrings)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MakeRecurringJob(className: invalidClassName)
            );
            Assert.Equal("className", exception.ParamName);
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'className')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given count argument
    /// is negative.
    /// </summary>
    [Fact]
    public void Constructor_Count_ThrowsWhenNegative()
    {
        foreach (var negativeValue in new int[] { -100, -1 })
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => MakeRecurringJob(count: Maybe<int>.Some(negativeValue))
            );
            Assert.Equal("count", exception.ParamName);
            Assert.Equal(
                $"""
                Value must be a non-negative whole number. (Parameter 'count')
                Actual value was {negativeValue}.
                """,
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
            () => MakeRecurringJob(data: null)
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
        foreach (var invalidData in TestConstants.EmptyStrings)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MakeRecurringJob(data: invalidData)
            );
            Assert.Equal("data", exception.ParamName);
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'data')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given intervalSeconds argument
    /// is not positive.
    /// </summary>
    [Fact]
    public void Constructor_IntervalSeconds_ThrowsWhenNegative()
    {
        foreach (var nonPositiveValue in new int[] { -100, -1, 0 })
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => MakeRecurringJob(intervalSeconds: Maybe<int>.Some(nonPositiveValue))
            );
            Assert.Equal("intervalSeconds", exception.ParamName);
            Assert.Equal(
                $"""
                Value must be a positive whole number. (Parameter 'intervalSeconds')
                Actual value was {nonPositiveValue}.
                """,
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given jid argument is
    /// null.
    /// </summary>
    [Fact]
    public void Constructor_Jid_ThrowsWhenNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeRecurringJob(jid: null)
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
        foreach (var invalidJid in TestConstants.EmptyStrings)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MakeRecurringJob(jid: invalidJid)
            );
            Assert.Equal("jid", exception.ParamName);
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'jid')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given maximumBacklog argument
    /// is negative.
    /// </summary>
    [Fact]
    public void Constructor_MaximumBacklog_ThrowsWhenNegative()
    {
        foreach (var negativeValue in new int[] { -100, -1 })
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => MakeRecurringJob(maximumBacklog: Maybe<int>.Some(negativeValue))
            );
            Assert.Equal("maximumBacklog", exception.ParamName);
            Assert.Equal(
                $"""
                Value must be a non-negative whole number. (Parameter 'maximumBacklog')
                Actual value was {negativeValue}.
                """,
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
                () => MakeRecurringJob(priority: Maybe<int>.Some(negativeValue))
            );
            Assert.Equal("priority", exception.ParamName);
            Assert.Equal(
                $"""
                Value must be a non-negative whole number. (Parameter 'priority')
                Actual value was {negativeValue}.
                """,
                exception.Message
            );
        }
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
        foreach (var invalidQueueName in TestConstants.EmptyStrings)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MakeRecurringJob(queueName: invalidQueueName)
            );
            Assert.Equal("queueName", exception.ParamName);
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'queueName')",
                exception.Message
            );
        }
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
                () => MakeRecurringJob(retries: Maybe<int>.Some(negativeValue))
            );
            Assert.Equal("retries", exception.ParamName);
            Assert.Equal(
                $"""
                Value must be a non-negative whole number. (Parameter 'retries')
                Actual value was {negativeValue}.
                """,
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
            () => MakeRecurringJob(state: null)
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
        foreach (var invalidState in TestConstants.EmptyStrings)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MakeRecurringJob(state: invalidState)
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
            () => MakeRecurringJob(tags: Maybe<string[]?>.Some(null))
        );
        Assert.Equal("tags", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'tags')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an exception if the given tags include
    /// values that are null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Tags_ThrowsWhenAnyValueIsNullEmptyOrOnlyWhitespace()
    {
        foreach (var invalidValue in TestConstants.EmptyStringsWithNull)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MakeRecurringJob(tags: Maybe<string[]?>.Some([invalidValue!]))
            );
            Assert.Equal("tags", exception.ParamName);
            Assert.Equal(
                "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'tags')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should throw an exception if the given throttles
    /// argument is null.
    /// </summary>
    [Fact]
    public void Constructor_Throttles_ThrowsWhenNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => MakeRecurringJob(throttles: Maybe<string[]?>.Some(null))
        );
        Assert.Equal("throttles", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'throttles')", exception.Message);
    }

    /// <summary>
    /// The constructor should throw an exception if the given throttles include
    /// values that are null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Throttles_ThrowsWhenAnyValueIsNullEmptyOrOnlyWhitespace()
    {
        foreach (var invalidValue in TestConstants.EmptyStringsWithNull)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MakeRecurringJob(throttles: Maybe<string[]?>.Some([invalidValue!]))
            );
            Assert.Equal("throttles", exception.ParamName);
            Assert.Equal(
                "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'throttles')",
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