using Reqless.Client.Models;
using Reqless.Client.Serialization;
using System.Text.Json;

namespace Reqless.Tests.Client.Serialization;

/// <summary>
/// Tests for the <see cref="JidsResultJsonConverter"/> class.
/// </summary>
public class JidsResultJsonConverterTest
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new JidsResultJsonConverter() },
    };

    /// <summary>
    /// <see cref="JidsResultJsonConverter.Read"/> should throw if the given
    /// JSON isn't an object.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonDoesNotStartWithObjectStart()
    {
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<JidsResult>(
                "[]",
                _jsonSerializerOptions));
        Assert.Equal(
            "Expected reader to begin with start of object.", exception.Message);
    }

    /// <summary>
    /// <see cref="JidsResultJsonConverter.Read"/> should throw if the given
    /// JSON has a 'jobs' property that is a non-empty object.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonHasJobsPropertyThatIsNonEmptyObject()
    {
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<JidsResult>(
                """{"jobs": {"key": "boom"}, "total": 4}""", _jsonSerializerOptions));
        Assert.Equal(
            "Expected 'jobs' to be array or empty object but encountered object with 1 properties.",
            exception.Message);
    }

    /// <summary>
    /// <see cref="JidsResultJsonConverter.Read"/> should throw if the given
    /// JSON has a 'jobs' property that cannot be deserialize into a string[].
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonHasJobsPropertyThatCannotBeDeserializedToStringArray()
    {
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<JidsResult>(
                """{"jobs": null, "total": 4}""", _jsonSerializerOptions));
        Assert.Equal(
            "Failed to deserialize 'jobs' property into a string[].",
            exception.Message);
    }

    /// <summary>
    /// <see cref="JidsResultJsonConverter.Read"/> should throw if the given
    /// JSON doesn't include a 'jobs' property.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonDoesNotIncludeJobsProperty()
    {
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<JidsResult>(
                """{"total": 0}""", _jsonSerializerOptions));
        Assert.Equal(
            "Expected 'jobs' property in JSON object, but none was found.",
            exception.Message);
    }

    /// <summary>
    /// <see cref="JidsResultJsonConverter.Read"/> should throw if the given
    /// JSON doesn't include a 'total' property.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonDoesNotIncludeTotalProperty()
    {
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<JidsResult>(
                """{"jobs": []}""", _jsonSerializerOptions));
        Assert.Equal(
            "Expected 'total' property in JSON object, but none was found.",
            exception.Message);
    }

    /// <summary>
    /// <see cref="JidsResultJsonConverter.Read"/> returns the expected <see
    /// cref="JidsResult"/> instance when given valid JSON.
    /// </summary>
    [Fact]
    public void Read_ReturnsExpectedJidsResult()
    {
        var jid = "jid1";
        var total = 1;
        var result = JsonSerializer.Deserialize<JidsResult>(
            $$"""{"jobs": ["{{jid}}"], "total": {{total}}}""", _jsonSerializerOptions);
        Assert.NotNull(result);
        Assert.Single(result.Jids);
        Assert.Equal(jid, result.Jids[0]);
        Assert.Equal(total, result.Total);
    }

    /// <summary>
    /// <see cref="JidsResultJsonConverter.Read"/> handles a jobs property that
    /// contains an empty JSON object like an empty array.
    /// </summary>
    [Fact]
    public void Read_HandlesJobsPropertyWithEmptyObjectLikeEmptyArray()
    {
        var result = JsonSerializer.Deserialize<JidsResult>(
            """{"jobs": {}, "total": 0}""", _jsonSerializerOptions);
        Assert.NotNull(result);
        Assert.Empty(result.Jids);
        Assert.Equal(0, result.Total);
    }

    /// <summary>
    /// <see cref="JidsResultJsonConverter.Write"/> can serialize a JidsResult
    /// instance so it can be round-tripped.
    /// </summary>
    [Fact]
    public void Write_CanSerializeJidsResultInstance()
    {
        var total = 4;
        var jids = new[] { "jid1", "jid2" };
        var subject = new JidsResult(jids, total);
        var json = JsonSerializer.Serialize(subject, _jsonSerializerOptions);
        var subjectFromJson = JsonSerializer.Deserialize<JidsResult>(
            json, _jsonSerializerOptions);
        Assert.NotNull(subjectFromJson);
        Assert.Equal(subject.Jids, subjectFromJson.Jids);
        Assert.Equal(subject.Total, subjectFromJson.Total);
    }
}
