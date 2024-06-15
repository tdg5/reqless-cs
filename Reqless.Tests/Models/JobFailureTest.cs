using Reqless.Models;
using System.Text.Json;

namespace Reqless.Tests.Models;

/// <summary>
/// Unit tests for <see cref="JobFailure"/>.
/// </summary>
public class JobFailureTest
{
    static readonly string[] EMPTY_STRINGS = ["", " ", "\t"];

    /// <summary>
    /// Constructor should throw when group is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGroupIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new JobFailure(null!, "message", 123, "workerName")
        );
        Assert.Equal("group", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'group')", exception.Message);
    }

    /// <summary>
    /// Constructor should throw when group is empty or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGroupIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in EMPTY_STRINGS)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => new JobFailure(emptyString, "message", 123, "workerName")
            );
            Assert.Equal("group", exception.ParamName);
            Assert.Equal("The value cannot be an empty string or composed entirely of whitespace. (Parameter 'group')", exception.Message);
        }
    }

    /// <summary>
    /// Constructor should throw when message is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenMessageIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new JobFailure("group", null!, 123, "workerName")
        );
        Assert.Equal("message", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'message')", exception.Message);
    }

    /// <summary>
    /// Constructor should throw when message is empty or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenMessageIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in EMPTY_STRINGS)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => new JobFailure("group", emptyString, 123, "workerName")
            );
            Assert.Equal("message", exception.ParamName);
            Assert.Equal("The value cannot be an empty string or composed entirely of whitespace. (Parameter 'message')", exception.Message);
        }
    }

    /// <summary>
    /// Constructor should throw when when is not positive.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenWhenIsNotPositive()
    {
        foreach (var invalidWhen in new int[] { 0, -1, -100 })
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new JobFailure("group", "message", invalidWhen, "workerName")
            );
            Assert.Equal("when", exception.ParamName);
            Assert.Equal("When must be greater than 0. (Parameter 'when')", exception.Message);
        }
    }

    /// <summary>
    /// Constructor should throw when workerName is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenWorkerNameIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new JobFailure("group", "message", 123, null!)
        );
        Assert.Equal("workerName", exception.ParamName);
        Assert.Equal("Value cannot be null. (Parameter 'workerName')", exception.Message);
    }

    /// <summary>
    /// Constructor should throw when workerName is empty or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenWorkerNameIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in EMPTY_STRINGS)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => new JobFailure("group", "message", 123, emptyString)
            );
            Assert.Equal("workerName", exception.ParamName);
            Assert.Equal("The value cannot be an empty string or composed entirely of whitespace. (Parameter 'workerName')", exception.Message);
        }
    }

    /// <summary>
    /// An instance should serialize to expected JSON.
    /// </summary>
    [Fact]
    public void SerializesToExpectedJson()
    {
        var subject = new JobFailure("group", "message", 123, "workerName");
        var jobFailureJson = JsonSerializer.Serialize(subject);
        var jobFailureFromJson = JsonSerializer.Deserialize<JobFailure>(jobFailureJson);
        Assert.NotNull(jobFailureFromJson);
        Assert.Equal(subject.Group, jobFailureFromJson.Group);
        Assert.Equal(subject.Message, jobFailureFromJson.Message);
        Assert.Equal(subject.When, jobFailureFromJson.When);
        Assert.Equal(subject.WorkerName, jobFailureFromJson.WorkerName);
    }

    /// <summary>
    /// An instance should deserialize from JSON as expected.
    /// </summary>
    [Fact]
    public void DeserializesFromJsonAsExpected()
    {
        var group = "group";
        var message = "message";
        var when = 123;
        var workerName = "workerName";
        var jobFailureJson = $$"""
            {
                "group": "{{group}}",
                "message": "{{message}}",
                "when": {{when}}, 
                "worker": "{{workerName}}"
            }
            """;
        var jobFailureFromJson = JsonSerializer.Deserialize<JobFailure>(jobFailureJson);
        Assert.NotNull(jobFailureFromJson);
        Assert.Equal(group, jobFailureFromJson.Group);
        Assert.Equal(message, jobFailureFromJson.Message);
        Assert.Equal(when, jobFailureFromJson.When);
        Assert.Equal(workerName, jobFailureFromJson.WorkerName);
    }
}