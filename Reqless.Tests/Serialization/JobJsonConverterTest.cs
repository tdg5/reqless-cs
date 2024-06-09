using Reqless.Models;
using Reqless.Models.JobEvents;
using Reqless.Tests.TestHelpers;
using System.Text;
using System.Text.Json;

namespace Reqless.Tests.Serialization;

/// <summary>
/// Tests for the <see cref="Reqless.Serialization.JobJsonConverter"/> class.
/// </summary>
public class JobJsonConverterTest
{
    /// <summary>
    /// Read should throw an error if the JSON is missing the data property.
    /// </summary>
    [Fact]
    public void Read_Data_ThrowsWhenOmitted()
    {
        var json = JobJson(data: Maybe<string?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'data' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null data property.
    /// </summary>
    [Fact]
    public void Read_Data_ThrowsWhenNull()
    {
        var json = JobJson(data: Maybe<string?>.Some(null));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Value cannot be null. (Parameter 'data')", exception.Message);
    }

    /// <summary>
    /// Read deserializes the data, keeping it as a string.
    /// </summary>
    [Fact]
    public void Read_Data_ResultsInADataString()
    {
        var data = "{}";
        var json = JobJson(data: Maybe<string?>.Some(data));
        var job = JsonSerializer.Deserialize<Job>(json);
        Assert.NotNull(job);
        Assert.Equal(data, job.Data);
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the dependencies
    /// property.
    /// </summary>
    [Fact]
    public void Read_Dependencies_ThrowsWhenOmitted()
    {
        var json = JobJson(dependencies: Maybe<string[]?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'dependencies' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null dependencies property.
    /// </summary>
    [Fact]
    public void Read_Dependencies_ThrowsWhenNull()
    {
        var json = JobJson(dependencies: Maybe<string[]?>.Some(null));
        var exception = Assert.Throws<ArgumentNullException>(
            () => JsonSerializer.Deserialize<Job>(json)
        );
        Assert.Equal("Value cannot be null. (Parameter 'dependencies')", exception.Message);
    }

    /// <summary>
    /// Read should throw if it encounters a dependencies property with a value
    /// that is an object that is not empty.
    /// </summary>
    [Fact]
    public void Read_Dependencies_ThrowsIfANonEmptyObjectIsEncountered()
    {
        var json = JobJsonRaw(
            dependencies: Maybe<string>.Some("""{"boom": "boom"}""")
        );
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Job>(json)
        );
        Assert.Equal(
            "Expected 'dependencies' to be array or empty object but encountered object with 1 properties.",
            exception.Message
        );
    }

    /// <summary>
    /// Read should be able to handle a dependencies property with a value that
    /// is an empty object and translate it to an empty array.
    /// </summary>
    [Fact]
    public void Read_Dependencies_TranslatesEmptyObjectToEmptyArray()
    {
        var json = JobJsonRaw(dependencies: Maybe<string>.Some("{}"));
        var job = JsonSerializer.Deserialize<Job>(json);
        Assert.NotNull(job);
        Assert.Equivalent(Array.Empty<string>(), job.Dependencies);
    }

    /// <summary>
    /// Read can handle a variety of valid values for the dependencies property.
    /// </summary>
    [Fact]
    public void Read_Dependencies_HandlesValidValues()
    {
        foreach (var validDependencies in new string[][] {
            [],
            ["1"],
            ["1", "2", "3", "4", "5"],
        })
        {
            var json = JobJson(dependencies: Maybe<string[]?>.Some(validDependencies));
            var job = JsonSerializer.Deserialize<Job>(json);
            Assert.NotNull(job);
            Assert.Equivalent(validDependencies, job.Dependencies);
        }
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the dependents
    /// property.
    /// </summary>
    [Fact]
    public void Read_Dependents_ThrowsWhenOmitted()
    {
        var json = JobJson(dependents: Maybe<string[]?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'dependents' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null dependents property.
    /// </summary>
    [Fact]
    public void Read_Dependents_ThrowsWhenNull()
    {
        var json = JobJson(dependents: Maybe<string[]?>.Some(null));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Value cannot be null. (Parameter 'dependents')", exception.Message);
    }

    /// <summary>
    /// Read should be able to handle a dependents property with a value that is
    /// an empty object and translate it to an empty array.
    /// </summary>
    [Fact]
    public void Read_Dependents_TranslatesEmptyObjectToEmptyArray()
    {
        var json = JobJsonRaw(dependents: Maybe<string>.Some("{}"));
        var job = JsonSerializer.Deserialize<Job>(json);
        Assert.NotNull(job);
        Assert.Equivalent(Array.Empty<string>(), job.Dependents);
    }

    /// <summary>
    /// Read should throw if it encounters a dependents property with a value
    /// that is an object that is not empty.
    /// </summary>
    [Fact]
    public void Read_Dependents_ThrowsIfANonEmptyObjectIsEncountered()
    {
        var json = JobJsonRaw(
            dependents: Maybe<string>.Some("""{"boom": "boom"}""")
        );
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Job>(json)
        );
        Assert.Equal(
            "Expected 'dependents' to be array or empty object but encountered object with 1 properties.",
            exception.Message
        );
    }

    /// <summary>
    /// Read can handle a variety of valid values for the dependencies property.
    /// </summary>
    [Fact]
    public void Read_Dependents_HandlesValidValues()
    {
        foreach (var validDependents in new string[][] {
            [],
            ["1"],
            ["1", "2", "3", "4", "5"],
        })
        {
            var json = JobJson(dependents: Maybe<string[]?>.Some(validDependents));
            var job = JsonSerializer.Deserialize<Job>(json);
            Assert.NotNull(job);
            Assert.Equivalent(validDependents, job.Dependents);
        }
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the expires property.
    /// </summary>
    [Fact]
    public void Read_Expires_ThrowsWhenOmitted()
    {
        var json = JobJson(expires: Maybe<long?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'expires' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null expires property.
    /// </summary>
    [Fact]
    public void Read_Expires_ThrowsWhenNull()
    {
        var json = JobJson(expires: Maybe<long?>.Some(null));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
    }

    /// <summary>
    /// Read should be able to handle a failure property with a value that is
    /// an empty object and translate it to null.
    /// </summary>
    [Fact]
    public void Read_Failure_TranslatesEmptyObjectToNull()
    {
        var json = JobJsonRaw(failure: Maybe<string>.Some("{}"));
        var job = JsonSerializer.Deserialize<Job>(json);
        Assert.NotNull(job);
        Assert.Null(job.Failure);
    }

    /// <summary>
    /// Read should allow failure to be null.
    /// </summary>
    [Fact]
    public void Read_Failure_CanBeNull()
    {
        var json = JobJson(failure: Maybe<JobFailure?>.Some(null));
        var job = JsonSerializer.Deserialize<Job>(json);
        Assert.NotNull(job);
        Assert.Null(job.Failure);
    }

    /// <summary>
    /// Read should be able to handle a history property with a value that is an
    /// empty object and translate it to an empty array.
    /// </summary>
    [Fact]
    public void Read_History_TranslatesEmptyObjectToEmptyArray()
    {
        var json = JobJsonRaw(history: Maybe<string>.Some("{}"));
        var job = JsonSerializer.Deserialize<Job>(json);
        Assert.NotNull(job);
        Assert.Equivalent(Array.Empty<string>(), job.History);
    }

    /// <summary>
    /// Read should throw if it encounters a history property with a value that
    /// is an object that is not empty.
    /// </summary>
    [Fact]
    public void Read_History_ThrowsIfANonEmptyObjectIsEncountered()
    {
        var json = JobJsonRaw(
            history: Maybe<string>.Some("""{"boom": "boom"}""")
        );
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Job>(json)
        );
        Assert.Equal(
            "Expected 'history' to be array or empty object but encountered object with 1 properties.",
            exception.Message
        );
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the jid property.
    /// </summary>
    [Fact]
    public void Read_Jid_ThrowsWhenOmitted()
    {
        var json = JobJson(jid: Maybe<string?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'jid' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null jid property.
    /// </summary>
    [Fact]
    public void Read_Jid_ThrowsWhenNull()
    {
        var json = JobJson(jid: Maybe<string?>.Some(null));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Value cannot be null. (Parameter 'jid')", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the priority property.
    /// </summary>
    [Fact]
    public void Read_Priority_ThrowsWhenOmitted()
    {
        var json = JobJson(priority: Maybe<int?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'priority' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null priority property.
    /// </summary>
    [Fact]
    public void Read_Priority_ThrowsWhenNull()
    {
        var json = JobJson(priority: Maybe<int?>.Some(null));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the queue property.
    /// </summary>
    [Fact]
    public void Read_Queue_ThrowsWhenOmitted()
    {
        var json = JobJson(queueName: Maybe<string?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'queue' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null queue property.
    /// </summary>
    [Fact]
    public void Read_Queue_ThrowsWhenNull()
    {
        var json = JobJson(queueName: Maybe<string?>.Some(null));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Value cannot be null. (Parameter 'queueName')", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the remaining
    /// property.
    /// </summary>
    [Fact]
    public void Read_Remaining_ThrowsWhenOmitted()
    {
        var json = JobJson(remaining: Maybe<int?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'remaining' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null remaining property.
    /// </summary>
    [Fact]
    public void Read_Remaining_ThrowsWhenNull()
    {
        var json = JobJson(remaining: Maybe<int?>.Some(null));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the retries property.
    /// </summary>
    [Fact]
    public void Read_Retries_ThrowsWhenOmitted()
    {
        var json = JobJson(retries: Maybe<int?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'retries' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null retries property.
    /// </summary>
    [Fact]
    public void Read_Retries_ThrowsWhenNull()
    {
        var json = JobJson(retries: Maybe<int?>.Some(null));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the spawned_from_jid
    /// property.
    /// </summary>
    [Fact]
    public void Read_SpawnedFromJid_ThrowsWhenOmitted()
    {
        var json = JobJson(spawnedFromJid: Maybe<string?>.None);
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Job>(json)
        );
        Assert.Equal(
            "Required property 'spawned_from_jid' not found.",
            exception.Message
        );
    }

    /// <summary>
    /// Read interprets a false spawned_from_jid property as a null value.
    /// </summary>
    [Fact]
    public void Read_SpawnedFromJid_TreatsFalseAsNull()
    {
        var json = JobJsonRaw(spawnedFromJid: Maybe<string>.Some("false"));
        var job = JsonSerializer.Deserialize<Job>(json);
        Assert.NotNull(job);
        Assert.Null(job.SpawnedFromJid);
    }

    /// <summary>
    /// Read interprets a null spawned_from_jid property as a null value.
    /// </summary>
    [Fact]
    public void Read_SpawnedFromJid_TreatsNullAsNull()
    {
        var json = JobJson(spawnedFromJid: Maybe<string?>.Some(null));
        var job = JsonSerializer.Deserialize<Job>(json);
        Assert.NotNull(job);
        Assert.Null(job.SpawnedFromJid);
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the state property.
    /// </summary>
    [Fact]
    public void Read_State_ThrowsWhenOmitted()
    {
        var json = JobJson(state: Maybe<string?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'state' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null state property.
    /// </summary>
    [Fact]
    public void Read_State_ThrowsWhenNull()
    {
        var json = JobJson(state: Maybe<string?>.Some(null));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Value cannot be null. (Parameter 'state')", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the tags property.
    /// </summary>
    [Fact]
    public void Read_Tags_ThrowsWhenOmitted()
    {
        var json = JobJson(tags: Maybe<string[]?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'tags' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null tags property.
    /// </summary>
    [Fact]
    public void Read_Tags_ThrowsWhenNull()
    {
        var json = JobJson(tags: Maybe<string[]?>.Some(null));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Value cannot be null. (Parameter 'tags')", exception.Message);
    }

    /// <summary>
    /// Read should throw if it encounters a tags property with a value that is
    /// an object that is not empty.
    /// </summary>
    [Fact]
    public void Read_Tags_ThrowsIfANonEmptyObjectIsEncountered()
    {
        var json = JobJsonRaw(
            tags: Maybe<string>.Some("""{"boom": "boom"}""")
        );
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Job>(json)
        );
        Assert.Equal(
            "Expected 'tags' to be array or empty object but encountered object with 1 properties.",
            exception.Message
        );
    }

    /// <summary>
    /// Read should be able to handle a tags property with a value that
    /// is an empty object and translate it to an empty array.
    /// </summary>
    [Fact]
    public void Read_Tags_TranslatesEmptyObjectToEmptyArray()
    {
        var json = JobJsonRaw(tags: Maybe<string>.Some("{}"));
        var job = JsonSerializer.Deserialize<Job>(json);
        Assert.NotNull(job);
        Assert.Equivalent(Array.Empty<string>(), job.Tags);
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the throttles
    /// property.
    /// </summary>
    [Fact]
    public void Read_Throttles_ThrowsWhenOmitted()
    {
        var json = JobJson(throttles: Maybe<string[]?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'throttles' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null throttles property.
    /// </summary>
    [Fact]
    public void Read_Throttles_ThrowsWhenNull()
    {
        var json = JobJson(throttles: Maybe<string[]?>.Some(null));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Value cannot be null. (Parameter 'throttles')", exception.Message);
    }

    /// <summary>
    /// Read should throw if it encounters a throttles property with a value
    /// that is an object that is not empty.
    /// </summary>
    [Fact]
    public void Read_Throttles_ThrowsIfANonEmptyObjectIsEncountered()
    {
        var json = JobJsonRaw(
            throttles: Maybe<string>.Some("""{"boom": "boom"}""")
        );
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Job>(json)
        );
        Assert.Equal(
            "Expected 'throttles' to be array or empty object but encountered object with 1 properties.",
            exception.Message
        );
    }

    /// <summary>
    /// Read should be able to handle a throttles property with a value that
    /// is an empty object and translate it to an empty array.
    /// </summary>
    [Fact]
    public void Read_Throttles_TranslatesEmptyObjectToEmptyArray()
    {
        var json = JobJsonRaw(throttles: Maybe<string>.Some("{}"));
        var job = JsonSerializer.Deserialize<Job>(json);
        Assert.NotNull(job);
        Assert.Equivalent(Array.Empty<string>(), job.Throttles);
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the tracked
    /// property.
    /// </summary>
    [Fact]
    public void Read_Tracked_ThrowsWhenOmitted()
    {
        var json = JobJson(tracked: Maybe<bool?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'tracked' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null tracked property.
    /// </summary>
    [Fact]
    public void Read_Tracked_ThrowsWhenNull()
    {
        var json = JobJson(tracked: Maybe<bool?>.Some(null));
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Job>(json)
        );
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the worker
    /// property.
    /// </summary>
    [Fact]
    public void Read_Worker_ThrowsWhenOmitted()
    {
        var json = JobJson(workerName: Maybe<string?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'worker' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null worker property.
    /// </summary>
    [Fact]
    public void Read_Worker_ThrowsWhenNull()
    {
        var json = JobJson(workerName: Maybe<string?>.Some(null));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Value cannot be null. (Parameter 'workerName')", exception.Message);
    }

    /// <summary>
    /// Read should ignore unknown properties of any type.
    /// </summary>
    [Fact]
    public void Read_IgnoresUnknownPropertiesOfAnyType()
    {
        foreach (var unknown in new string[] {
            "1",
            "3.14",
            "[]",
            "\"unknown\"",
            "false",
            "null",
            "{}",
        })
        {
            var json = JobJsonRaw(unknown: Maybe<string>.Some(unknown));
            var job = JsonSerializer.Deserialize<Job>(json);
            Assert.NotNull(job);
        }
    }

    /// <summary>
    /// Read should be able to deserialize a job with all properties set.
    /// </summary>
    [Fact]
    public void Read_CanDeserializeJob()
    {
        string className = "className";
        string data = "{}";
        string[] dependencies = ["dependency"];
        string[] dependents = ["dependent"];
        long expires = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 60000;
        var failure = new JobFailure(
            group: "group",
            message: "message",
            when: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            workerName: "workerName"
        );
        var history = new JobEvent[] {
            new PutEvent(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "queue"),
            new DoneEvent(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
        };
        string jid = "jid";
        int priority = 0;
        string queueName = "queue";
        int remaining = 0;
        int retries = 0;
        string state = "state";
        string[] tags = ["tag"];
        string[] throttles = ["throttle"];
        string workerName = "workerName";
        var json = JobJson(
            className: Maybe<string?>.Some(className),
            data: Maybe<string?>.Some(data),
            dependencies: Maybe<string[]?>.Some(dependencies),
            dependents: Maybe<string[]?>.Some(dependents),
            expires: Maybe<long?>.Some(expires),
            failure: Maybe<JobFailure?>.Some(failure),
            history: Maybe<JobEvent[]?>.Some(history),
            jid: Maybe<string?>.Some(jid),
            priority: Maybe<int?>.Some(priority),
            queueName: Maybe<string?>.Some(queueName),
            remaining: Maybe<int?>.Some(remaining),
            retries: Maybe<int?>.Some(retries),
            state: Maybe<string?>.Some(state),
            tags: Maybe<string[]?>.Some(tags),
            throttles: Maybe<string[]?>.Some(throttles),
            workerName: Maybe<string?>.Some(workerName)
        );

        var job = JsonSerializer.Deserialize<Job>(json);
        Assert.NotNull(job);
        Assert.Equal(className, job.ClassName);
        Assert.Equal(data, job.Data);
        Assert.Equal(dependencies, job.Dependencies);
        Assert.Equal(dependents, job.Dependents);
        Assert.Equal(expires, job.Expires);
        Assert.Equivalent(failure, job.Failure);
        Assert.Equivalent(history, job.History);
        Assert.Equal(jid, job.Jid);
        Assert.Equal(priority, job.Priority);
        Assert.Equal(queueName, job.QueueName);
        Assert.Equal(remaining, job.Remaining);
        Assert.Equal(retries, job.Retries);
        Assert.Equal(state, job.State);
        Assert.Equal(tags, job.Tags);
        Assert.Equal(throttles, job.Throttles);
        Assert.Equal(workerName, job.WorkerName);
    }

    /// <summary>
    /// Write should be able to serialize a job with all properties set.
    /// </summary>
    [Fact]
    public void Write_CanSerializeJob()
    {
        var className = "klass";
        var data = "{}";
        var dependencies = new string[] { "dependency" };
        var dependents = new string[] { "dependent" };
        var expires = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var failure = new JobFailure(
            group: "group",
            message: "message",
            when: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            workerName: "workerName"
        );
        var history = new JobEvent[] {
            new PutEvent(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "queue"),
        };
        var jid = "jid";
        var priority = 25;
        var queueName = "queueName";
        var remaining = 5;
        var retries = 6;
        var spawnedFromJid = "spawnedFromJid";
        var state = "state";
        var tags = new string[] { "tag" };
        var throttles = new string[] { "throttle" };
        var tracked = false;
        var workerName = "workerName";

        var job = new Job(
            className: className,
            data: data,
            dependencies: dependencies,
            dependents: dependents,
            expires: expires,
            failure: failure,
            history: history,
            jid: jid,
            priority: priority,
            queueName: queueName,
            remaining: remaining,
            retries: retries,
            spawnedFromJid: spawnedFromJid,
            state: state,
            tags: tags,
            throttles: throttles,
            tracked: tracked,
            workerName: workerName
        );
        var json = JsonSerializer.Serialize(job);
        var expectedJobJson = JobJson(
            className: Maybe<string?>.Some(className),
            data: Maybe<string?>.Some(data),
            dependencies: Maybe<string[]?>.Some(dependencies),
            dependents: Maybe<string[]?>.Some(dependents),
            expires: Maybe<long?>.Some(expires),
            failure: Maybe<JobFailure?>.Some(failure),
            history: Maybe<JobEvent[]?>.Some(history),
            jid: Maybe<string?>.Some(jid),
            priority: Maybe<int?>.Some(priority),
            queueName: Maybe<string?>.Some(queueName),
            remaining: Maybe<int?>.Some(remaining),
            retries: Maybe<int?>.Some(retries),
            spawnedFromJid: Maybe<string?>.Some(spawnedFromJid),
            state: Maybe<string?>.Some(state),
            tags: Maybe<string[]?>.Some(tags),
            throttles: Maybe<string[]?>.Some(throttles),
            tracked: Maybe<bool?>.Some(tracked),
            workerName: Maybe<string?>.Some(workerName)
        );
        Assert.Equal(expectedJobJson, json);
    }

    /// <summary>
    /// Returns a JSON string representing a job.
    /// </summary>
    /// <remarks>
    /// For the purposes of this method, for each argument, a given value of
    /// null will get a default value, a value of Maybe{T}.None will cause the
    /// respective property to be omitted from the output, and a value of
    /// Maybe{T}.Some(...) will cause the given value be rendered in the output.
    /// </remarks>
    static string JobJson(
        Maybe<string?>? className = null,
        Maybe<string?>? data = null,
        Maybe<string[]?>? dependencies = null,
        Maybe<string[]?>? dependents = null,
        Maybe<long?>? expires = null,
        Maybe<JobFailure?>? failure = null,
        Maybe<JobEvent[]?>? history = null,
        Maybe<string?>? jid = null,
        Maybe<int?>? priority = null,
        Maybe<string?>? queueName = null,
        Maybe<int?>? remaining = null,
        Maybe<int?>? retries = null,
        Maybe<string?>? spawnedFromJid = null,
        Maybe<string?>? state = null,
        Maybe<string[]?>? tags = null,
        Maybe<string[]?>? throttles = null,
        Maybe<bool?>? tracked = null,
        Maybe<string?>? workerName = null
    )
    {
        var jsonValueMaybes = new Dictionary<string, Maybe<string>?>();

        jsonValueMaybes["className"] = (className ?? Maybe<string?>.Some("className"))
            .Map(value => value is null ? "null" : $"\"{value}\"");

        jsonValueMaybes["data"] = (data ?? Maybe<string?>.Some("{}"))
            .Map(value => value is null ? $"null" : $"\"{value}\"");

        jsonValueMaybes["dependencies"] = (dependencies ?? Maybe<string[]?>.Some([]))
            .Map(value => JsonSerializer.Serialize(value));

        jsonValueMaybes["dependents"] = (dependents ?? Maybe<string[]?>.Some([]))
            .Map(value => JsonSerializer.Serialize(value));

        jsonValueMaybes["expires"] =
            (
                expires
                ?? Maybe<long?>.Some(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
            )
            .Map<string>(value => value?.ToString() ?? "null");

        jsonValueMaybes["failure"] =
            (
                failure ?? Maybe<JobFailure?>.Some(
                    new JobFailure(
                        group: "group",
                        message: "message",
                        when: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                        workerName: "workerName"
                    )
                )
            )
            .Map(value => JsonSerializer.Serialize(value));

        jsonValueMaybes["history"] = (history ?? Maybe<JobEvent[]?>.Some([]))
            .Map(value => JsonSerializer.Serialize(value));

        jsonValueMaybes["jid"] = (jid ?? Maybe<string?>.Some("jid"))
            .Map(value => value is null ? "null" : $"\"{value}\"");

        jsonValueMaybes["priority"] = (priority ?? Maybe<int?>.Some(25))
            .Map(value => value?.ToString() ?? "null");

        jsonValueMaybes["queueName"] = (queueName ?? Maybe<string?>.Some("queueName"))
            .Map(value => value is null ? "null" : $"\"{value}\"");

        jsonValueMaybes["remaining"] = (remaining ?? Maybe<int?>.Some(5))
            .Map(value => value?.ToString() ?? "null");

        jsonValueMaybes["retries"] = (retries ?? Maybe<int?>.Some(6))
            .Map(value => value?.ToString() ?? "null");

        jsonValueMaybes["spawnedFromJid"] = (spawnedFromJid ?? Maybe<string?>.Some("spawnedFromJid"))
            .Map(value => value is null ? "null" : $"\"{value}\"");

        jsonValueMaybes["state"] = (state ?? Maybe<string?>.Some("state"))
            .Map(value => value is null ? "null" : $"\"{value}\"");

        jsonValueMaybes["tags"] = (tags ?? Maybe<string[]?>.Some([]))
            .Map(value => JsonSerializer.Serialize(value));

        jsonValueMaybes["throttles"] = (throttles ?? Maybe<string[]?>.Some([]))
            .Map(value => JsonSerializer.Serialize(value));

        jsonValueMaybes["tracked"] = (tracked ?? Maybe<bool?>.Some(false))
            .Map(value => value is null ? "null" : value == true ? "true" : "false");

        jsonValueMaybes["workerName"] = (workerName ?? Maybe<string?>.Some("workerName"))
            .Map(value => value is null ? "null" : $"\"{value}\"");

        return JobJsonRaw(
            className: jsonValueMaybes["className"],
            data: jsonValueMaybes["data"],
            dependencies: jsonValueMaybes["dependencies"],
            dependents: jsonValueMaybes["dependents"],
            expires: jsonValueMaybes["expires"],
            failure: jsonValueMaybes["failure"],
            history: jsonValueMaybes["history"],
            jid: jsonValueMaybes["jid"],
            priority: jsonValueMaybes["priority"],
            queueName: jsonValueMaybes["queueName"],
            remaining: jsonValueMaybes["remaining"],
            retries: jsonValueMaybes["retries"],
            spawnedFromJid: jsonValueMaybes["spawnedFromJid"],
            state: jsonValueMaybes["state"],
            tags: jsonValueMaybes["tags"],
            throttles: jsonValueMaybes["throttles"],
            tracked: jsonValueMaybes["tracked"],
            workerName: jsonValueMaybes["workerName"]
        );
    }

    /// <summary>
    /// Returns a JSON string representing a job. All arguments are expected to
    /// have been serialized to JSON format already.
    /// </summary>
    static string JobJsonRaw(
        Maybe<string>? className = null,
        Maybe<string>? data = null,
        Maybe<string>? dependencies = null,
        Maybe<string>? dependents = null,
        Maybe<string>? expires = null,
        Maybe<string>? failure = null,
        Maybe<string>? history = null,
        Maybe<string>? jid = null,
        Maybe<string>? priority = null,
        Maybe<string>? queueName = null,
        Maybe<string>? remaining = null,
        Maybe<string>? retries = null,
        Maybe<string>? spawnedFromJid = null,
        Maybe<string>? state = null,
        Maybe<string>? tags = null,
        Maybe<string>? throttles = null,
        Maybe<string>? tracked = null,
        Maybe<string>? unknown = null,
        Maybe<string>? workerName = null
    )
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var classNameMaybe = className ?? Maybe<string>.Some("\"className\"");
        var dataMaybe = data ?? Maybe<string>.Some("\"{}\"");
        var dependenciesMaybe = dependencies ?? Maybe<string>.Some("[]");
        var dependentsMaybe = dependents ?? Maybe<string>.Some("[]");
        var expiresMaybe = expires ?? Maybe<string>.Some((now + 60000).ToString());
        var failureMaybe = failure ?? Maybe<string>.Some(
            JsonSerializer.Serialize(
                new JobFailure(
                    group: "group",
                    message: "message",
                    when: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    workerName: "workerName"
                )
            )
        );
        var historyMaybe = history ?? Maybe<string>.Some("[]");
        var jidMaybe = jid ?? Maybe<string>.Some("\"jid\"");
        var priorityMaybe = priority ?? Maybe<string>.Some("25");
        var queueNameMaybe = queueName ?? Maybe<string>.Some("\"queueName\"");
        var remainingMaybe = remaining ?? Maybe<string>.Some("5");
        var retriesMaybe = retries ?? Maybe<string>.Some("6");
        var spawnedFromJidMaybe = spawnedFromJid ?? Maybe<string>.Some("\"spawnedFromJid\"");
        var stateMaybe = state ?? Maybe<string>.Some("\"state\"");
        var tagsMaybe = tags ?? Maybe<string>.Some("[]");
        var throttlesMaybe = throttles ?? Maybe<string>.Some("[]");
        var trackedMaybe = tracked ?? Maybe<string>.Some("false");
        var unknownMaybe = unknown ?? Maybe<string>.None;
        var workerNameMaybe = workerName ?? Maybe<string>.Some("\"workerName\"");

        var json = new StringBuilder();
        json.Append('{');
        if (dataMaybe.HasValue)
        {
            var dataValueJson = dataMaybe.GetOrDefault("{}");
            json.Append($"\"data\":{dataValueJson},");
        }
        if (dependenciesMaybe.HasValue)
        {
            var dependenciesValueJson = dependenciesMaybe.GetOrDefault("[]");
            json.Append($"\"dependencies\":{dependenciesValueJson},");
        }
        if (dependentsMaybe.HasValue)
        {
            var dependentsValueJson = dependentsMaybe.GetOrDefault("[]");
            json.Append($"\"dependents\":{dependentsValueJson},");
        }
        if (expiresMaybe.HasValue)
        {
            var expiresValueJson = expiresMaybe.GetOrDefault("12345");
            json.Append($"\"expires\":{expiresValueJson},");
        }
        if (failureMaybe.HasValue)
        {
            var failureValueJson = failureMaybe.GetOrDefault("null");
            json.Append($"\"failure\":{failureValueJson},");
        }
        if (historyMaybe.HasValue)
        {
            var historyValueJson = historyMaybe.GetOrDefault("[]");
            json.Append($"\"history\":{historyValueJson},");
        }
        if (jidMaybe.HasValue)
        {
            var jidValueJson = jidMaybe.GetOrDefault("\"jid\"");
            json.Append($"\"jid\":{jidValueJson},");
        }
        if (classNameMaybe.HasValue)
        {
            var classNameValueJson = classNameMaybe.GetOrDefault("\"className\"");
            json.Append($"\"klass\":{classNameValueJson},");
        }
        if (priorityMaybe.HasValue)
        {
            var priorityValueJson = priorityMaybe.GetOrDefault("12345");
            json.Append($"\"priority\":{priorityValueJson},");
        }
        if (queueNameMaybe.HasValue)
        {
            var queueNameValueJson = queueNameMaybe.GetOrDefault("\"queueName\"");
            json.Append($"\"queue\":{queueNameValueJson},");
        }
        if (remainingMaybe.HasValue)
        {
            var remainingValueJson = remainingMaybe.GetOrDefault("6");
            json.Append($"\"remaining\":{remainingValueJson},");
        }
        if (retriesMaybe.HasValue)
        {
            var retriesValueJson = retriesMaybe.GetOrDefault("5");
            json.Append($"\"retries\":{retriesValueJson},");
        }
        if (spawnedFromJidMaybe.HasValue)
        {
            var spawnedFromJidValueJson = spawnedFromJidMaybe.GetOrDefault("\"spawnedFromJid\"");
            json.Append($"\"spawned_from_jid\":{spawnedFromJidValueJson},");
        }
        if (stateMaybe.HasValue)
        {
            var stateValueJson = stateMaybe.GetOrDefault("\"running\"");
            json.Append($"\"state\":{stateValueJson},");
        }
        if (tagsMaybe.HasValue)
        {
            var tagsJson = tagsMaybe.GetOrDefault("[]");
            json.Append($"\"tags\":{tagsJson},");
        }
        if (throttlesMaybe.HasValue)
        {
            var throttlesJson = throttlesMaybe.GetOrDefault("[]");
            json.Append($"\"throttles\":{throttlesJson},");
        }
        if (trackedMaybe.HasValue)
        {
            var trackedValueJson = trackedMaybe.GetOrDefault("false");
            json.Append($"\"tracked\":{trackedValueJson},");
        }
        if (unknownMaybe.HasValue)
        {
            var unknownValueJson = unknownMaybe.GetOrDefault("null");
            json.Append($"\"unknown\":{unknownValueJson},");
        }
        if (workerNameMaybe.HasValue)
        {
            var workerNameValueJson = workerNameMaybe.GetOrDefault("\"workerName\"");
            json.Append($"\"worker\":{workerNameValueJson},");
        }
        json.Remove(json.Length - 1, 1);
        json.Append('}');
        return json.ToString();
    }
}