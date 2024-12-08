using Reqless.Client.Models;
using Reqless.Client.Serialization;
using Reqless.Common.Utilities;
using Reqless.Tests.Common.Client.Models;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

namespace Reqless.Tests.Client.Serialization;

/// <summary>
/// Tests for the <see cref="JobJsonConverter"/> class.
/// </summary>
public class JobJsonConverterTest
{
    /// <summary>
    /// Read should throw an error if the JSON is missing the data property.
    /// </summary>
    [Fact]
    public void Read_Data_ThrowsWhenOmitted()
    {
        var json = JobFactory.JobJson(data: Maybe<string>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'data' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null data property.
    /// </summary>
    [Fact]
    public void Read_Data_ThrowsWhenNull()
    {
        var json = JobFactory.JobJson(data: Maybe<string>.Some(null!));
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
        var json = JobFactory.JobJson(data: Maybe.Some(data));
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
        var json = JobFactory.JobJson(dependencies: Maybe<string[]>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'dependencies' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null dependencies property.
    /// </summary>
    [Fact]
    public void Read_Dependencies_ThrowsWhenNull()
    {
        var json = JobFactory.JobJson(dependencies: Maybe<string[]>.Some(null!));
        var exception = Assert.Throws<ArgumentNullException>(
            () => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Value cannot be null. (Parameter 'dependencies')", exception.Message);
    }

    /// <summary>
    /// Read should throw if it encounters a dependencies property with a value
    /// that is an object that is not empty.
    /// </summary>
    [Fact]
    public void Read_Dependencies_ThrowsIfANonEmptyObjectIsEncountered()
    {
        var json = JobFactory.JobJsonRaw(
            dependencies: Maybe.Some("""{"boom": "boom"}"""));
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal(
            "Expected 'dependencies' to be array or empty object but encountered object with 1 properties.",
            exception.Message);
    }

    /// <summary>
    /// Read should be able to handle a dependencies property with a value that
    /// is an empty object and translate it to an empty array.
    /// </summary>
    [Fact]
    public void Read_Dependencies_TranslatesEmptyObjectToEmptyArray()
    {
        var json = JobFactory.JobJsonRaw(dependencies: Maybe.Some("{}"));
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
        var dependencySets = new string[][]
        {
            [],
            ["1"],
            ["1", "2", "3", "4", "5"],
        };
        foreach (var validDependencies in dependencySets)
        {
            var json = JobFactory.JobJson(dependencies: Maybe.Some(validDependencies));
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
        var json = JobFactory.JobJson(dependents: Maybe<string[]>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'dependents' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null dependents property.
    /// </summary>
    [Fact]
    public void Read_Dependents_ThrowsWhenNull()
    {
        var json = JobFactory.JobJson(dependents: Maybe<string[]>.Some(null!));
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
        var json = JobFactory.JobJsonRaw(dependents: Maybe.Some("{}"));
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
        var json = JobFactory.JobJsonRaw(
            dependents: Maybe.Some("""{"boom": "boom"}"""));
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal(
            "Expected 'dependents' to be array or empty object but encountered object with 1 properties.",
            exception.Message);
    }

    /// <summary>
    /// Read can handle a variety of valid values for the dependencies property.
    /// </summary>
    [Fact]
    public void Read_Dependents_HandlesValidValues()
    {
        var dependentsSets = new string[][]
        {
            [],
            ["1"],
            ["1", "2", "3", "4", "5"],
        };
        foreach (var validDependents in dependentsSets)
        {
            var json = JobFactory.JobJson(dependents: Maybe.Some(validDependents));
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
        var json = JobFactory.JobJson(expires: Maybe<long?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'expires' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null expires property.
    /// </summary>
    [Fact]
    public void Read_Expires_ThrowsWhenNull()
    {
        var json = JobFactory.JobJson(expires: Maybe<long?>.Some(null));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
    }

    /// <summary>
    /// Read handles a non-positive expires by converting it to null.
    /// </summary>
    [Fact]
    public void Read_Expires_NonPositiveValuesAreConvertedToNull()
    {
        foreach (var nonPositiveExpires in new long[] { 0, -1, -100 })
        {
            var json = JobFactory.JobJson(expires: Maybe<long?>.Some(nonPositiveExpires));
            var job = JsonSerializer.Deserialize<Job>(json);
            Assert.NotNull(job);
            Assert.Null(job.Expires);
        }
    }

    /// <summary>
    /// Read handles a positive expires value by keeping it as is.
    /// </summary>
    [Fact]
    public void Read_Expires_PositiveValuesAreKeptAsIs()
    {
        foreach (var positiveExpires in new long[] { 1, 100 })
        {
            var json = JobFactory.JobJson(expires: Maybe<long?>.Some(positiveExpires));
            var job = JsonSerializer.Deserialize<Job>(json);
            Assert.NotNull(job);
            Assert.Equal(positiveExpires, job.Expires);
        }
    }

    /// <summary>
    /// Read should be able to handle a failure property with a value that is
    /// an empty object and translate it to null.
    /// </summary>
    [Fact]
    public void Read_Failure_TranslatesEmptyObjectToNull()
    {
        var json = JobFactory.JobJsonRaw(failure: Maybe.Some("{}"));
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
        var json = JobFactory.JobJson(failure: Maybe<JobFailure>.Some(null!));
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
        var json = JobFactory.JobJsonRaw(history: Maybe.Some("{}"));
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
        var json = JobFactory.JobJsonRaw(
            history: Maybe.Some("""{"boom": "boom"}"""));
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal(
            "Expected 'history' to be array or empty object but encountered object with 1 properties.",
            exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the jid property.
    /// </summary>
    [Fact]
    public void Read_Jid_ThrowsWhenOmitted()
    {
        var json = JobFactory.JobJson(jid: Maybe<string>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'jid' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null jid property.
    /// </summary>
    [Fact]
    public void Read_Jid_ThrowsWhenNull()
    {
        var json = JobFactory.JobJson(jid: Maybe<string>.Some(null!));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Value cannot be null. (Parameter 'jid')", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the priority property.
    /// </summary>
    [Fact]
    public void Read_Priority_ThrowsWhenOmitted()
    {
        var json = JobFactory.JobJson(priority: Maybe<int?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'priority' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null priority property.
    /// </summary>
    [Fact]
    public void Read_Priority_ThrowsWhenNull()
    {
        var json = JobFactory.JobJson(priority: Maybe<int?>.Some(null));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the queue property.
    /// </summary>
    [Fact]
    public void Read_Queue_ThrowsWhenOmitted()
    {
        var json = JobFactory.JobJson(queueName: Maybe<string>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'queue' not found.", exception.Message);
    }

    /// <summary>
    /// Read should allow queue/queueName to be null.
    /// </summary>
    [Fact]
    public void Read_Queue_CanBeNull()
    {
        string json = JobFactory.JobJson(queueName: Maybe<string>.Some(null!));
        Job? job = JsonSerializer.Deserialize<Job>(json);
        Assert.NotNull(job);
        Assert.Null(job.QueueName);
    }

    /// <summary>
    /// Read should handle empty and whitespace queue/queueName as null.
    /// </summary>
    [Fact]
    public void Read_Queue_EmptyOrWhitespaceIsConveretedToNull()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            string json = JobFactory.JobJson(queueName: Maybe.Some(emptyString));
            Job? job = JsonSerializer.Deserialize<Job>(json);
            Assert.NotNull(job);
            Assert.Null(job.QueueName);
        }
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the remaining
    /// property.
    /// </summary>
    [Fact]
    public void Read_Remaining_ThrowsWhenOmitted()
    {
        var json = JobFactory.JobJson(remaining: Maybe<int?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'remaining' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null remaining property.
    /// </summary>
    [Fact]
    public void Read_Remaining_ThrowsWhenNull()
    {
        var json = JobFactory.JobJson(remaining: Maybe<int?>.Some(null));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the retries property.
    /// </summary>
    [Fact]
    public void Read_Retries_ThrowsWhenOmitted()
    {
        var json = JobFactory.JobJson(retries: Maybe<int?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'retries' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null retries property.
    /// </summary>
    [Fact]
    public void Read_Retries_ThrowsWhenNull()
    {
        var json = JobFactory.JobJson(retries: Maybe<int?>.Some(null));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the spawned_from_jid
    /// property.
    /// </summary>
    [Fact]
    public void Read_SpawnedFromJid_ThrowsWhenOmitted()
    {
        var json = JobFactory.JobJson(spawnedFromJid: Maybe<string>.None);
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal(
            "Required property 'spawned_from_jid' not found.", exception.Message);
    }

    /// <summary>
    /// Read interprets a false spawned_from_jid property as a null value.
    /// </summary>
    [Fact]
    public void Read_SpawnedFromJid_TreatsFalseAsNull()
    {
        var json = JobFactory.JobJsonRaw(spawnedFromJid: Maybe.Some("false"));
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
        var json = JobFactory.JobJson(spawnedFromJid: Maybe<string>.Some(null!));
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
        var json = JobFactory.JobJson(state: Maybe<string>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'state' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null state property.
    /// </summary>
    [Fact]
    public void Read_State_ThrowsWhenNull()
    {
        var json = JobFactory.JobJson(state: Maybe<string>.Some(null!));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Value cannot be null. (Parameter 'state')", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the tags property.
    /// </summary>
    [Fact]
    public void Read_Tags_ThrowsWhenOmitted()
    {
        var json = JobFactory.JobJson(tags: Maybe<string[]>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'tags' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null tags property.
    /// </summary>
    [Fact]
    public void Read_Tags_ThrowsWhenNull()
    {
        var json = JobFactory.JobJson(tags: Maybe<string[]>.Some(null!));
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
        var json = JobFactory.JobJsonRaw(
            tags: Maybe.Some("""{"boom": "boom"}"""));
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal(
            "Expected 'tags' to be array or empty object but encountered object with 1 properties.",
            exception.Message);
    }

    /// <summary>
    /// Read should be able to handle a tags property with a value that
    /// is an empty object and translate it to an empty array.
    /// </summary>
    [Fact]
    public void Read_Tags_TranslatesEmptyObjectToEmptyArray()
    {
        var json = JobFactory.JobJsonRaw(tags: Maybe.Some("{}"));
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
        var json = JobFactory.JobJson(throttles: Maybe<string[]>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'throttles' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null throttles property.
    /// </summary>
    [Fact]
    public void Read_Throttles_ThrowsWhenNull()
    {
        var json = JobFactory.JobJson(throttles: Maybe<string[]>.Some(null!));
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
        var json = JobFactory.JobJsonRaw(
            throttles: Maybe.Some("""{"boom": "boom"}"""));
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal(
            "Expected 'throttles' to be array or empty object but encountered object with 1 properties.",
            exception.Message);
    }

    /// <summary>
    /// Read should be able to handle a throttles property with a value that
    /// is an empty object and translate it to an empty array.
    /// </summary>
    [Fact]
    public void Read_Throttles_TranslatesEmptyObjectToEmptyArray()
    {
        var json = JobFactory.JobJsonRaw(throttles: Maybe.Some("{}"));
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
        var json = JobFactory.JobJson(tracked: Maybe<bool?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'tracked' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null tracked property.
    /// </summary>
    [Fact]
    public void Read_Tracked_ThrowsWhenNull()
    {
        var json = JobFactory.JobJson(tracked: Maybe<bool?>.Some(null));
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Job>(json));
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the worker
    /// property.
    /// </summary>
    [Fact]
    public void Read_Worker_ThrowsWhenOmitted()
    {
        var json = JobFactory.JobJson(workerName: Maybe<string>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Job>(json));
        Assert.Equal("Required property 'worker' not found.", exception.Message);
    }

    /// <summary>
    /// Read should allow worker to have a null value.
    /// </summary>
    [Fact]
    public void Read_Worker_ToleratesNull()
    {
        var json = JobFactory.JobJson(workerName: Maybe<string>.Some(null!));
        var job = JsonSerializer.Deserialize<Job>(json);
        Assert.NotNull(job);
        Assert.Null(job.WorkerName);
    }

    /// <summary>
    /// Read should ignore unknown properties of any type.
    /// </summary>
    [Fact]
    public void Read_IgnoresUnknownPropertiesOfAnyType()
    {
        var unknownValues = new string[]
        {
            "1",
            "3.14",
            "[]",
            "\"unknown\"",
            "false",
            "null",
            "{}",
        };
        foreach (var unknownValue in unknownValues)
        {
            var json = JobFactory.JobJsonRaw(unknown: Maybe.Some(unknownValue));
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
        long expires = GetNow() + 60000;
        var failure = new JobFailure(
            group: "group",
            message: "message",
            when: GetNow(),
            workerName: "workerName");
        var history = new JobEvent[]
        {
            new PutEvent(GetNow(), "queue"),
            new DoneEvent(GetNow()),
        };
        string jid = "jid";
        int priority = 0;
        string queueName = "queue";
        int remaining = 0;
        int retries = 0;
        string state = "waiting";
        string[] tags = ["tag"];
        string[] throttles = ["throttle"];
        string workerName = "workerName";
        var json = JobFactory.JobJson(
            className: Maybe.Some(className),
            data: Maybe.Some(data),
            dependencies: Maybe.Some(dependencies),
            dependents: Maybe.Some(dependents),
            expires: Maybe<long?>.Some(expires),
            failure: Maybe.Some(failure),
            history: Maybe.Some(history),
            jid: Maybe.Some(jid),
            priority: Maybe<int?>.Some(priority),
            queueName: Maybe.Some(queueName),
            remaining: Maybe<int?>.Some(remaining),
            retries: Maybe<int?>.Some(retries),
            state: Maybe.Some(state),
            tags: Maybe.Some(tags),
            throttles: Maybe.Some(throttles),
            workerName: Maybe.Some(workerName));

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
        var expires = GetNow();
        var failure = new JobFailure(
            group: "group",
            message: "message",
            when: GetNow(),
            workerName: "workerName");
        var history = new JobEvent[] { new PutEvent(GetNow(), "queue") };
        var jid = "jid";
        var priority = 25;
        var queueName = "queueName";
        var remaining = 5;
        var retries = 6;
        var spawnedFromJid = "spawnedFromJid";
        var state = "waiting";
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
            workerName: workerName);
        var json = JsonSerializer.Serialize(job);
        var expectedJobJson = JobFactory.JobJson(
            className: Maybe.Some(className),
            data: Maybe.Some(data),
            dependencies: Maybe.Some(dependencies),
            dependents: Maybe.Some(dependents),
            expires: Maybe<long?>.Some(expires),
            failure: Maybe.Some(failure),
            history: Maybe.Some(history),
            jid: Maybe.Some(jid),
            priority: Maybe<int?>.Some(priority),
            queueName: Maybe.Some(queueName),
            remaining: Maybe<int?>.Some(remaining),
            retries: Maybe<int?>.Some(retries),
            spawnedFromJid: Maybe.Some(spawnedFromJid),
            state: Maybe.Some(state),
            tags: Maybe.Some(tags),
            throttles: Maybe.Some(throttles),
            tracked: Maybe<bool?>.Some(tracked),
            workerName: Maybe.Some(workerName));
        Assert.Equal(expectedJobJson, json);
    }

    /// <summary>
    /// Write serializes a null Expires value as 0.
    /// </summary>
    [Fact]
    public void Write_Expires_SerializesNullAsZero()
    {
        var job = JobFactory.NewJob(expires: Maybe<long?>.Some(null));
        var json = JsonSerializer.Serialize(job);
        Assert.Contains("\"expires\":0", json);
    }

    /// <summary>
    /// Write serializes a null Failure value as null.
    /// </summary>
    [Fact]
    public void Write_Failure_SerializesNullCorrectly()
    {
        var job = JobFactory.NewJob(failure: Maybe<JobFailure>.Some(null!));
        var json = JsonSerializer.Serialize(job);
        Assert.Contains("\"failure\":null", json);
    }

    private static long GetNow()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
