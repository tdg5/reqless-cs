using Reqless.Models.JobEvents;
using Reqless.Serialization;
using System.Text.Json;

namespace Reqless.Tests.Serialization;

/// <summary>
/// Tests for <see cref="JobEventJsonConverter"/>.
/// </summary>
public class JobEventJsonConverterTest
{
    JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        Converters = { new JobEventJsonConverter() }
    };

    /// <summary>
    /// Read should throw if the given JSON isn't an object.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonDoesNotStartWithObjectStart()
    {
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<JobEvent>(
                "[]",
                _jsonSerializerOptions
            )
        );
        Assert.Equal(
            "Expected reader to begin with start of object.",
            exception.Message
        );
    }

    /// <summary>
    /// Read should throw if the given JSON object has a null "what" property.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonHasNullWhatProperty()
    {
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<JobEvent>(
                """{"when": 123, "what": null}""",
                _jsonSerializerOptions
            )
        );
        Assert.Equal(
            "Expected a string value for the 'what' property, got null.",
            exception.Message
        );
    }

    /// <summary>
    /// Read should throw if the given JSON object doesn't have a "what" property.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonDoesNotIncludeWhatProperty()
    {
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<JobEvent>(
                """{"when": 123}""",
                _jsonSerializerOptions
            )
        );
        Assert.Equal(
            "Expected 'what' property in JSON object, but none was found.",
            exception.Message
        );
    }

    /// <summary>
    /// Read should throw if the given JSON object contains a "what" property
    /// with an unknown value.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonIncludesUnknownWhatProperty()
    {
        var what = "unknown";
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<JobEvent>(
                $$"""{"what": "{{what}}", "when": 123}""",
                _jsonSerializerOptions
            )
        );
        Assert.Equal(
            $"Unknown job event type: {what}.",
            exception.Message
        );
    }

    /// <summary>
    /// Read should only read "what" from the root of the JSON object.
    /// </summary>
    [Fact]
    public void Read_CanDetermineWhatInSpiteOfNestedStructures()
    {
        var jobEvent = JsonSerializer.Deserialize<JobEvent>(
            """
            {
                "arrayWithDone": [{"what": "failed"}, {"what": null}],
                "objectWithDone": {"what": "failed"},
                "when": 123,
                "what": "done"
            }
            """,
            _jsonSerializerOptions
        );
        Assert.NotNull(jobEvent);
        Assert.Equal("done", jobEvent.What);
    }

    /// <summary>
    /// Read should be able to deserialize a <see cref="DoneEvent"/>  event.
    /// </summary>
    [Fact]
    public void Read_CanDeserializeDoneEvent()
    {
        var when = 123;
        var jobEvent = JsonSerializer.Deserialize<JobEvent>(
            $$"""{"what": "done", "when": {{when}}}""",
            _jsonSerializerOptions
        );
        Assert.NotNull(jobEvent);
        Assert.Equal("done", jobEvent.What);
        Assert.Equal(when, jobEvent.When);
        Assert.IsType<DoneEvent>(jobEvent);
    }

    /// <summary>
    /// Read should be able to deserialize a <see cref="FailedEvent"/>  event.
    /// </summary>
    [Fact]
    public void Read_CanDeserializeFailedEvent()
    {
        var group = "group";
        var when = 123;
        var workerName = "workerName";
        var jobEvent = JsonSerializer.Deserialize<JobEvent>(
            $$"""
            {
                "group": "{{group}}",
                "what": "failed",
                "when": {{when}},
                "worker": "{{workerName}}"
            }
            """,
            _jsonSerializerOptions
        );
        Assert.NotNull(jobEvent);
        Assert.Equal("failed", jobEvent.What);
        Assert.Equal(when, jobEvent.When);
        var failedEvent = Assert.IsType<FailedEvent>(jobEvent);
        Assert.Equal(group, failedEvent.Group);
        Assert.Equal(workerName, failedEvent.WorkerName);
    }

    /// <summary>
    /// Read should be able to deserialize a <see cref="FailedRetriesEvent"/>
    /// event.
    /// </summary>
    [Fact]
    public void Read_CanDeserializeFailedRetriesEvent()
    {
        var group = "group";
        var when = 123;
        var jobEvent = JsonSerializer.Deserialize<JobEvent>(
            $$"""
            {
                "group": "{{group}}",
                "what": "failed-retries",
                "when": {{when}}
            }
            """,
            _jsonSerializerOptions
        );
        Assert.NotNull(jobEvent);
        Assert.Equal("failed-retries", jobEvent.What);
        Assert.Equal(when, jobEvent.When);
        var failedRetriesEvent = Assert.IsType<FailedRetriesEvent>(jobEvent);
        Assert.Equal(group, failedRetriesEvent.Group);
    }

    /// <summary>
    /// Read should be able to deserialize a <see cref="PoppedEvent"/> event.
    /// </summary>
    [Fact]
    public void Read_CanDeserializePoppedEvent()
    {
        var workerName = "workerName";
        var when = 123;
        var jobEvent = JsonSerializer.Deserialize<JobEvent>(
            $$"""
            {
                "what": "popped",
                "when": {{when}},
                "worker": "{{workerName}}"
            }
            """,
            _jsonSerializerOptions
        );
        Assert.NotNull(jobEvent);
        Assert.Equal("popped", jobEvent.What);
        Assert.Equal(when, jobEvent.When);
        var poppedEvent = Assert.IsType<PoppedEvent>(jobEvent);
        Assert.Equal(workerName, poppedEvent.WorkerName);
    }

    /// <summary>
    /// Read should be able to deserialize a <see cref="PutEvent"/> event.
    /// </summary>
    [Fact]
    public void Read_CanDeserializePutEvent()
    {
        var queueName = "queue";
        var when = 123;
        var jobEvent = JsonSerializer.Deserialize<JobEvent>(
            $$"""
            {
                "queue": "{{queueName}}",
                "what": "put",
                "when": {{when}}
            }
            """,
            _jsonSerializerOptions
        );
        Assert.NotNull(jobEvent);
        Assert.Equal("put", jobEvent.What);
        Assert.Equal(when, jobEvent.When);
        var putEvent = Assert.IsType<PutEvent>(jobEvent);
        Assert.Equal(queueName, putEvent.QueueName);
    }

    /// <summary>
    /// Read should be able to deserialize a <see cref="ThrottledEvent"/> event.
    /// </summary>
    [Fact]
    public void Read_CanDeserializeThrottledEvent()
    {
        var queueName = "queue";
        var when = 123;
        var jobEvent = JsonSerializer.Deserialize<JobEvent>(
            $$"""
            {
                "queue": "{{queueName}}",
                "what": "throttled",
                "when": {{when}}
            }
            """,
            _jsonSerializerOptions
        );
        Assert.NotNull(jobEvent);
        Assert.Equal("throttled", jobEvent.What);
        Assert.Equal(when, jobEvent.When);
        var throttledEvent = Assert.IsType<ThrottledEvent>(jobEvent);
        Assert.Equal(queueName, throttledEvent.QueueName);
    }

    /// <summary>
    /// Read should be able to deserialize a <see cref="TimedOutEvent"/> event.
    /// </summary>
    [Fact]
    public void Read_CanDeserializeTimedOutEvent()
    {
        var when = 123;
        var jobEvent = JsonSerializer.Deserialize<JobEvent>(
            $$"""{"what": "timed-out", "when": {{when}}}""",
            _jsonSerializerOptions
        );
        Assert.NotNull(jobEvent);
        Assert.Equal("timed-out", jobEvent.What);
        Assert.Equal(when, jobEvent.When);
        Assert.IsType<TimedOutEvent>(jobEvent);
    }

    /// <summary>
    /// Write should be able to serialize a <see cref="DoneEvent"/> event.
    /// </summary>
    [Fact]
    public void Write_CanSerializeDoneEvent()
    {
        var when = 123;
        var doneEvent = new DoneEvent(when);
        var json = JsonSerializer.Serialize(doneEvent, _jsonSerializerOptions);
        var jobEventFromJson = JsonSerializer.Deserialize<JobEvent>(json, _jsonSerializerOptions);
        var doneEventFromJson = Assert.IsType<DoneEvent>(jobEventFromJson);
        Assert.Equal(doneEvent.What, doneEventFromJson.What);
        Assert.Equal(doneEvent.When, doneEventFromJson.When);
    }

    /// <summary>
    /// Write should be able to serialize a <see cref="FailedEvent"/> event.
    /// </summary>
    [Fact]
    public void Write_CanSerializeFailedEvent()
    {
        var group = "group";
        var when = 123;
        var workerName = "workerName";
        var failedEvent = new FailedEvent(when, group, workerName);
        var json = JsonSerializer.Serialize(failedEvent, _jsonSerializerOptions);
        var jobEventFromJson = JsonSerializer.Deserialize<JobEvent>(json, _jsonSerializerOptions);
        var failedEventFromJson = Assert.IsType<FailedEvent>(jobEventFromJson);
        Assert.Equal(failedEvent.What, failedEventFromJson.What);
        Assert.Equal(failedEvent.When, failedEventFromJson.When);
        Assert.Equal(failedEvent.Group, failedEventFromJson.Group);
        Assert.Equal(failedEvent.WorkerName, failedEventFromJson.WorkerName);
    }

    /// <summary>
    /// Write should be able to serialize a <see cref="FailedRetriesEvent"/> event.
    /// </summary>
    [Fact]
    public void Write_CanSerializeFailedRetriesEvent()
    {
        var group = "group";
        var when = 123;
        var failedRetriesEvent = new FailedRetriesEvent(when, group);
        var json = JsonSerializer.Serialize(failedRetriesEvent, _jsonSerializerOptions);
        var jobEventFromJson = JsonSerializer.Deserialize<JobEvent>(json, _jsonSerializerOptions);
        var failedRetriesEventFromJson = Assert.IsType<FailedRetriesEvent>(jobEventFromJson);
        Assert.Equal(failedRetriesEvent.What, failedRetriesEventFromJson.What);
        Assert.Equal(failedRetriesEvent.When, failedRetriesEventFromJson.When);
        Assert.Equal(failedRetriesEvent.Group, failedRetriesEventFromJson.Group);
    }

    /// <summary>
    /// Write should be able to serialize a <see cref="PoppedEvent"/> event.
    /// </summary>
    [Fact]
    public void Write_CanSerializePoppedEvent()
    {
        var when = 123;
        var workerName = "workerName";
        var poppedEvent = new PoppedEvent(when, workerName);
        var json = JsonSerializer.Serialize(poppedEvent, _jsonSerializerOptions);
        var jobEventFromJson = JsonSerializer.Deserialize<JobEvent>(json, _jsonSerializerOptions);
        var poppedEventFromJson = Assert.IsType<PoppedEvent>(jobEventFromJson);
        Assert.Equal(poppedEvent.What, poppedEventFromJson.What);
        Assert.Equal(poppedEvent.When, poppedEventFromJson.When);
        Assert.Equal(poppedEvent.WorkerName, poppedEventFromJson.WorkerName);
    }

    /// <summary>
    /// Write should be able to serialize a <see cref="PutEvent"/> event.
    /// </summary>
    [Fact]
    public void Write_CanSerializePutEvent()
    {
        var when = 123;
        var queueName = "queueName";
        var putEvent = new PutEvent(when, queueName);
        var json = JsonSerializer.Serialize(putEvent, _jsonSerializerOptions);
        var jobEventFromJson = JsonSerializer.Deserialize<JobEvent>(json, _jsonSerializerOptions);
        var putEventFromJson = Assert.IsType<PutEvent>(jobEventFromJson);
        Assert.Equal(putEvent.What, putEventFromJson.What);
        Assert.Equal(putEvent.When, putEventFromJson.When);
        Assert.Equal(putEvent.QueueName, putEventFromJson.QueueName);
    }

    /// <summary>
    /// Write should be able to serialize a <see cref="ThrottledEvent"/> event.
    /// </summary>
    [Fact]
    public void Write_CanSerializeThrottledEvent()
    {
        var when = 123;
        var queueName = "queueName";
        var throttledEvent = new ThrottledEvent(when, queueName);
        var json = JsonSerializer.Serialize(throttledEvent, _jsonSerializerOptions);
        var jobEventFromJson = JsonSerializer.Deserialize<JobEvent>(json, _jsonSerializerOptions);
        var throttledEventFromJson = Assert.IsType<ThrottledEvent>(jobEventFromJson);
        Assert.Equal(throttledEvent.What, throttledEventFromJson.What);
        Assert.Equal(throttledEvent.When, throttledEventFromJson.When);
        Assert.Equal(throttledEvent.QueueName, throttledEventFromJson.QueueName);
    }

    /// <summary>
    /// Write should be able to serialize a <see cref="TimedOutEvent"/> event.
    /// </summary>
    [Fact]
    public void Write_CanSerializeTimedOutEvent()
    {
        var when = 123;
        var timedOutEvent = new TimedOutEvent(when);
        var json = JsonSerializer.Serialize(timedOutEvent, _jsonSerializerOptions);
        var jobEventFromJson = JsonSerializer.Deserialize<JobEvent>(json, _jsonSerializerOptions);
        var timedOutEventFromJson = Assert.IsType<TimedOutEvent>(jobEventFromJson);
        Assert.Equal(timedOutEvent.What, timedOutEventFromJson.What);
        Assert.Equal(timedOutEvent.When, timedOutEventFromJson.When);
    }
}