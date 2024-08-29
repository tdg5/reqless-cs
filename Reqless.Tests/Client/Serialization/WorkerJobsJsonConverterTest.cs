using Reqless.Client.Models;
using Reqless.Client.Serialization;
using System.Text.Json;

namespace Reqless.Tests.Client.Serialization;

/// <summary>
/// Tests for the <see cref="WorkerJobsJsonConverter"/> class.
/// </summary>
public class WorkerJobsJsonConverterTest
{
    JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        Converters = { new JobEventJsonConverter() }
    };

    /// <summary>
    /// <see cref="WorkerJobsJsonConverter.Read"/> should throw if the
    /// JSON does not start with an object.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonDoesNotStartWithObject()
    {
        var json = "[]";
        var exception = Assert.Throws<JsonException>(() =>
        {
            JsonSerializer.Deserialize<WorkerJobs>(json, _jsonSerializerOptions);
        });
        Assert.Equal("Expected reader to begin with start of object.", exception.Message);
    }

    /// <summary>
    /// <see cref="WorkerJobsJsonConverter.Read"/> should throw if the
    /// JSON does not include a jobs property.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonDoesNotIncludeJobsProperty()
    {
        var json = """{"stalled": []}""";
        var exception = Assert.Throws<JsonException>(() =>
        {
            JsonSerializer.Deserialize<WorkerJobs>(json, _jsonSerializerOptions);
        });
        Assert.Equal("Expected 'jobs' property in JSON object, but none was found.", exception.Message);
    }

    /// <summary>
    /// <see cref="WorkerJobsJsonConverter.Read"/> should throw if the
    /// jobs property of the JSON can't be deserialized.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonIncludesInvalidJobsProperty()
    {
        var json = """{"stalled": [], "jobs": null}""";
        var exception = Assert.Throws<JsonException>(() =>
        {
            JsonSerializer.Deserialize<WorkerJobs>(json, _jsonSerializerOptions);
        });
        Assert.Equal("Failed to deserialize 'jobs' property into a string[].", exception.Message);
    }

    /// <summary>
    /// <see cref="WorkerJobsJsonConverter.Read"/> should handle jobs
    /// property with empty JSON object as though it's an empty array.
    /// </summary>
    [Fact]
    public void Read_HandlesJobsWithJsonObjectAsEmptyArray()
    {
        var json = """{"stalled": [], "jobs": {}}""";
        var job = JsonSerializer.Deserialize<WorkerJobs>(
            json,
            _jsonSerializerOptions
        );
        Assert.NotNull(job);
        Assert.Empty(job.Jobs);
    }

    /// <summary>
    /// <see cref="WorkerJobsJsonConverter.Read"/> should throw if the
    /// JSON does not include a stalled property.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonDoesNotIncludeStalledProperty()
    {
        var json = """{"jobs": []}""";
        var exception = Assert.Throws<JsonException>(() =>
        {
            JsonSerializer.Deserialize<WorkerJobs>(json, _jsonSerializerOptions);
        });
        Assert.Equal(
            "Expected 'stalled' property in JSON object, but none was found.",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="WorkerJobsJsonConverter.Read"/> should throw if the
    /// stalled property of the JSON can't be deserialized.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonIncludesInvalidStalledProperty()
    {
        var json = """{"stalled": null, "jobs": []}""";
        var exception = Assert.Throws<JsonException>(() =>
        {
            JsonSerializer.Deserialize<WorkerJobs>(json, _jsonSerializerOptions);
        });
        Assert.Equal(
            "Failed to deserialize 'stalled' property into a string[].",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="WorkerJobsJsonConverter.Read"/> should handle stalled
    /// property with empty JSON object as though it's an empty array.
    /// </summary>
    [Fact]
    public void Read_HandlesStalledWithJsonObjectAsEmptyArray()
    {
        var json = """{"stalled": {}, "jobs": []}""";
        var job = JsonSerializer.Deserialize<WorkerJobs>(
            json,
            _jsonSerializerOptions
        );
        Assert.NotNull(job);
        Assert.Empty(job.Stalled);
    }

    /// <summary>
    /// <see cref="WorkerJobsJsonConverter.Write"/> should write JSON
    /// that can be round-tripped.
    /// </summary>
    [Fact]
    public void Write_ProducesJsonThatCanBeRoundTripped()
    {
        var trackedJobsResult = new WorkerJobs(
            stalled: ["jid-1", "jid-2"],
            jobs: ["jid-3"]
        );
        var json = JsonSerializer.Serialize(trackedJobsResult, _jsonSerializerOptions);
        var deserialized = JsonSerializer.Deserialize<WorkerJobs>(json, _jsonSerializerOptions);
        Assert.NotNull(deserialized);
        Assert.Equivalent(trackedJobsResult.Jobs, deserialized.Jobs);
        Assert.Equal(trackedJobsResult.Stalled, deserialized.Stalled);
    }
}