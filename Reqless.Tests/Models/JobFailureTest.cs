using Reqless.Models;
using Reqless.Tests.TestHelpers;
using System.Text.Json;

namespace Reqless.Tests.Models;

/// <summary>
/// Unit tests for <see cref="JobFailure"/>.
/// </summary>
public class JobFailureTest
{
    /// <summary>
    /// Constructor should throw when group is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Group_ThrowsWhenGroupIsNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidGroup) => new JobFailure(
                group: invalidGroup!,
                message: "message",
                when: 123,
                workerName: "workerName"
            ),
            "group"
        );
    }

    /// <summary>
    /// Constructor should throw when message is null, empty, or only
    /// whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Message_ThrowsWhenNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidMessage) => new JobFailure(
                group: "group",
                message: invalidMessage!,
                when: 123,
                workerName: "workerName"
            ),
            "message"
        );
    }

    /// <summary>
    /// Constructor should throw when the when argument is not positive.
    /// </summary>
    [Fact]
    public void Constructor_When_ThrowsWhenNotPositive()
    {
        Scenario.ThrowsWhenArgumentIsNotPositive(
            (long invalidWhen) => new JobFailure(
                "group",
                "message",
                invalidWhen,
                "workerName"
            ),
            "when"
        );
    }

    /// <summary>
    /// Constructor should throw when workerName is null, empty, or only
    /// whitespace.
    /// </summary>
    [Fact]
    public void Constructor_WorkerName_ThrowsWhenNullOrEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidWorkerName) => new JobFailure(
                "group",
                "message",
                123,
                invalidWorkerName!
            ),
            "workerName"
        );
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