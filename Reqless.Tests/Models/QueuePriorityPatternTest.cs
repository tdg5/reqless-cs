using System.Text.Json;
using Reqless.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Models;

/// <summary>
/// Unit tests for <see cref="QueuePriorityPattern"/>.
/// </summary>
public class QueuePriorityPatternTest
{
    /// <summary>
    /// <see cref="QueuePriorityPattern"/> constructor should throw if pattern is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsIfPatternIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new QueuePriorityPattern(fairly: true, pattern: null!),
            "pattern"
        );
    }

    /// <summary>
    /// <see cref="QueuePriorityPattern"/> constructor should throw if any
    /// pattern value is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsIfAnyPatternValueIsNullEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespace(
            (invalidPattern) => new QueuePriorityPattern(
                fairly: true,
                pattern: ["valid", invalidPattern!, "other"]
            ),
            "pattern"
        );
    }

    /// <summary>
    /// <see cref="QueuePriorityPattern"/> constructor should set properties
    /// appropriately.
    /// </summary>
    [Fact]
    public void Constructor_SetsProperties()
    {
        var fairly = true;
        List<string> pattern = ["queue1", "queue2"];
        var subject = new QueuePriorityPattern(
            fairly: fairly,
            pattern: pattern
        );
        Assert.Equal(fairly, subject.Fairly);
        Assert.Equal(pattern, subject.Pattern);
    }

    /// <summary>
    /// <see cref="QueuePriorityPattern"/> constructor should default fairly to
    /// false.
    /// </summary>
    [Fact]
    public void Constructor_DefaultsFairlyToFalse()
    {
        List<string> pattern = ["queue1", "queue2"];
        var subject = new QueuePriorityPattern(pattern: pattern);
        Assert.False(subject.Fairly);
        Assert.Equal(pattern, subject.Pattern);
    }

    /// <summary>
    /// <see cref="QueuePriorityPattern"/> should deserialize from JSON as
    /// expected.
    /// </summary>
    [Fact]
    public void DeserializeFromJson()
    {
        var json = """{"fairly":true,"pattern":["queue1","queue2"]}""";
        var subject = JsonSerializer.Deserialize<QueuePriorityPattern>(json);
        Assert.NotNull(subject);
        Assert.True(subject.Fairly);
        Assert.Equal(["queue1", "queue2"], subject.Pattern);
    }

    /// <summary>
    /// <see cref="QueuePriorityPattern"/> should serialize to JSON in a way
    /// that can be round-tripped.
    /// </summary>
    [Fact]
    public void SerializeToJson()
    {
        var subject = new QueuePriorityPattern(
            fairly: true,
            pattern: ["queue1", "queue2"]
        );
        var json = JsonSerializer.Serialize(subject);
        var subjectFromJson = JsonSerializer.Deserialize<QueuePriorityPattern>(json);
        Assert.Equivalent(subject, subjectFromJson);
    }
}