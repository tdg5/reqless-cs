using Reqless.Client.Models;
using Reqless.Client.Serialization;
using Reqless.Tests.TestHelpers.Client.Models;
using System.Text.Json;

namespace Reqless.Tests.Client.Serialization;

/// <summary>
/// Tests for the <see cref="TrackedJobsResultJsonConverter"/> class.
/// </summary>
public class TrackedJobsResultJsonConverterTest
{
    JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        Converters = { new JobEventJsonConverter() }
    };

    /// <summary>
    /// <see cref="TrackedJobsResultJsonConverter.Read"/> should throw if the
    /// JSON does not start with an object.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonDoesNotStartWithObject()
    {
        var json = "[]";
        var exception = Assert.Throws<JsonException>(() =>
        {
            JsonSerializer.Deserialize<TrackedJobsResult>(json, _jsonSerializerOptions);
        });
        Assert.Equal("Expected reader to begin with start of object.", exception.Message);
    }

    /// <summary>
    /// <see cref="TrackedJobsResultJsonConverter.Read"/> should throw if the
    /// JSON does not include a jobs property.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonDoesNotIncludeJobsProperty()
    {
        var json = """{"expired": []}""";
        var exception = Assert.Throws<JsonException>(() =>
        {
            JsonSerializer.Deserialize<TrackedJobsResult>(json, _jsonSerializerOptions);
        });
        Assert.Equal("Expected 'jobs' property in JSON object, but none was found.", exception.Message);
    }

    /// <summary>
    /// <see cref="TrackedJobsResultJsonConverter.Read"/> should throw if the
    /// jobs property of the JSON can't be deserialized.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonIncludesInvalidJobsProperty()
    {
        var json = """{"expired": [], "jobs": null}""";
        var exception = Assert.Throws<JsonException>(() =>
        {
            JsonSerializer.Deserialize<TrackedJobsResult>(json, _jsonSerializerOptions);
        });
        Assert.Equal("Failed to deserialize 'jobs' property into a Job[].", exception.Message);
    }

    /// <summary>
    /// <see cref="TrackedJobsResultJsonConverter.Read"/> should handle jobs
    /// property with empty JSON object as though it's an empty array.
    /// </summary>
    [Fact]
    public void Read_HandlesJobsWithJsonObjectAsEmptyArray()
    {
        var json = """{"expired": [], "jobs": {}}""";
        var trackedJobs = JsonSerializer.Deserialize<TrackedJobsResult>(
            json,
            _jsonSerializerOptions
        );
        Assert.NotNull(trackedJobs);
        Assert.Empty(trackedJobs.Jobs);
    }

    /// <summary>
    /// <see cref="TrackedJobsResultJsonConverter.Read"/> should throw if the
    /// JSON does not include a expired property.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonDoesNotIncludeExpiredProperty()
    {
        var json = """{"jobs": []}""";
        var exception = Assert.Throws<JsonException>(() =>
        {
            JsonSerializer.Deserialize<TrackedJobsResult>(json, _jsonSerializerOptions);
        });
        Assert.Equal(
            "Expected 'expired' property in JSON object, but none was found.",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="TrackedJobsResultJsonConverter.Read"/> should throw if the
    /// expired property of the JSON can't be deserialized.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonIncludesInvalidExpiredProperty()
    {
        var json = """{"expired": null, "jobs": []}""";
        var exception = Assert.Throws<JsonException>(() =>
        {
            JsonSerializer.Deserialize<TrackedJobsResult>(json, _jsonSerializerOptions);
        });
        Assert.Equal(
            "Failed to deserialize 'expired' property into a string[].",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="TrackedJobsResultJsonConverter.Read"/> should handle expired
    /// property with empty JSON object as though it's an empty array.
    /// </summary>
    [Fact]
    public void Read_HandlesExpiredWithJsonObjectAsEmptyArray()
    {
        var json = """{"expired": {}, "jobs": []}""";
        var trackedJobs = JsonSerializer.Deserialize<TrackedJobsResult>(
            json,
            _jsonSerializerOptions
        );
        Assert.NotNull(trackedJobs);
        Assert.Empty(trackedJobs.ExpiredJids);
    }

    /// <summary>
    /// <see cref="TrackedJobsResultJsonConverter.Write"/> should write JSON
    /// that can be round-tripped.
    /// </summary>
    [Fact]
    public void Write_ProducesJsonThatCanBeRoundTripped()
    {
        var trackedJobsResult = new TrackedJobsResult(
            expiredJids: new string[] { "j1", "j2" },
            jobs: [JobFactory.NewJob()]
        );
        var json = JsonSerializer.Serialize(trackedJobsResult, _jsonSerializerOptions);
        var deserialized = JsonSerializer.Deserialize<TrackedJobsResult>(json, _jsonSerializerOptions);
        Assert.NotNull(deserialized);
        Assert.Equivalent(trackedJobsResult.Jobs, deserialized.Jobs);
        Assert.Equal(trackedJobsResult.ExpiredJids, deserialized.ExpiredJids);
    }
}
