using Reqless.Client.Models;
using Reqless.Client.Serialization;
using Reqless.Tests.TestHelpers;
using Reqless.Tests.TestHelpers.Factories;
using System.Text.Json;

namespace Reqless.Tests.Client.Serialization;

/// <summary>
/// Tests for the <see cref="RecurringJobJsonConverter"/>
/// class.
/// </summary>
public class RecurringRecurringJobJsonConverterTest
{
    /// <summary>
    /// Read should throw an error if the JSON is missing the data property.
    /// </summary>
    [Fact]
    public void Read_Data_ThrowsWhenOmitted()
    {
        var json = RecurringJobFactory.RecurringJobJson(data: Maybe<string?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
        Assert.Equal("Required property 'data' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null data property.
    /// </summary>
    [Fact]
    public void Read_Data_ThrowsWhenNull()
    {
        var json = RecurringJobFactory.RecurringJobJson(data: Maybe<string?>.Some(null));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
        Assert.Equal("Value cannot be null. (Parameter 'data')", exception.Message);
    }

    /// <summary>
    /// Read deserializes the data, keeping it as a string.
    /// </summary>
    [Fact]
    public void Read_Data_ResultsInADataString()
    {
        var data = "{}";
        var json = RecurringJobFactory.RecurringJobJson(data: Maybe<string?>.Some(data));
        var job = JsonSerializer.Deserialize<RecurringJob>(json);
        Assert.NotNull(job);
        Assert.Equal(data, job.Data);
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the jid property.
    /// </summary>
    [Fact]
    public void Read_Jid_ThrowsWhenOmitted()
    {
        var json = RecurringJobFactory.RecurringJobJson(jid: Maybe<string?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
        Assert.Equal("Required property 'jid' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null jid property.
    /// </summary>
    [Fact]
    public void Read_Jid_ThrowsWhenNull()
    {
        var json = RecurringJobFactory.RecurringJobJson(jid: Maybe<string?>.Some(null));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
        Assert.Equal("Value cannot be null. (Parameter 'jid')", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the priority property.
    /// </summary>
    [Fact]
    public void Read_Priority_ThrowsWhenOmitted()
    {
        var json = RecurringJobFactory.RecurringJobJson(priority: Maybe<int?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
        Assert.Equal("Required property 'priority' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null priority property.
    /// </summary>
    [Fact]
    public void Read_Priority_ThrowsWhenNull()
    {
        var json = RecurringJobFactory.RecurringJobJson(priority: Maybe<int?>.Some(null));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the queue property.
    /// </summary>
    [Fact]
    public void Read_Queue_ThrowsWhenOmitted()
    {
        var json = RecurringJobFactory.RecurringJobJson(queueName: Maybe<string?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
        Assert.Equal("Required property 'queue' not found.", exception.Message);
    }

    /// <summary>
    /// Read should allow queue/queueName to be null.
    /// </summary>
    [Fact]
    public void Read_Queue_CanBeNull()
    {
        string json = RecurringJobFactory.RecurringJobJson(queueName: Maybe<string?>.Some(null));
        RecurringJob? job = JsonSerializer.Deserialize<RecurringJob>(json);
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
            string json = RecurringJobFactory.RecurringJobJson(
                queueName: Maybe<string?>.Some(emptyString)
            );
            RecurringJob? job = JsonSerializer.Deserialize<RecurringJob>(json);
            Assert.NotNull(job);
            Assert.Null(job.QueueName);
        }
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the retries property.
    /// </summary>
    [Fact]
    public void Read_Retries_ThrowsWhenOmitted()
    {
        var json = RecurringJobFactory.RecurringJobJson(retries: Maybe<int?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
        Assert.Equal("Required property 'retries' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null retries property.
    /// </summary>
    [Fact]
    public void Read_Retries_ThrowsWhenNull()
    {
        var json = RecurringJobFactory.RecurringJobJson(retries: Maybe<int?>.Some(null));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the state property.
    /// </summary>
    [Fact]
    public void Read_State_ThrowsWhenOmitted()
    {
        var json = RecurringJobFactory.RecurringJobJson(state: Maybe<string?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
        Assert.Equal("Required property 'state' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null state property.
    /// </summary>
    [Fact]
    public void Read_State_ThrowsWhenNull()
    {
        var json = RecurringJobFactory.RecurringJobJson(state: Maybe<string?>.Some(null));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
        Assert.Equal("Value cannot be null. (Parameter 'state')", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON is missing the tags property.
    /// </summary>
    [Fact]
    public void Read_Tags_ThrowsWhenOmitted()
    {
        var json = RecurringJobFactory.RecurringJobJson(tags: Maybe<string[]?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
        Assert.Equal("Required property 'tags' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null tags property.
    /// </summary>
    [Fact]
    public void Read_Tags_ThrowsWhenNull()
    {
        var json = RecurringJobFactory.RecurringJobJson(tags: Maybe<string[]?>.Some(null));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
        Assert.Equal("Value cannot be null. (Parameter 'tags')", exception.Message);
    }

    /// <summary>
    /// Read should throw if it encounters a tags property with a value that is
    /// an object that is not empty.
    /// </summary>
    [Fact]
    public void Read_Tags_ThrowsIfANonEmptyObjectIsEncountered()
    {
        var json = RecurringJobFactory.RecurringJobJsonRaw(
            tags: Maybe<string>.Some("""{"boom": "boom"}""")
        );
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<RecurringJob>(json)
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
        var json = RecurringJobFactory.RecurringJobJsonRaw(tags: Maybe<string>.Some("{}"));
        var job = JsonSerializer.Deserialize<RecurringJob>(json);
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
        var json = RecurringJobFactory.RecurringJobJson(throttles: Maybe<string[]?>.None);
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
        Assert.Equal("Required property 'throttles' not found.", exception.Message);
    }

    /// <summary>
    /// Read should throw an error if the JSON has a null throttles property.
    /// </summary>
    [Fact]
    public void Read_Throttles_ThrowsWhenNull()
    {
        var json = RecurringJobFactory.RecurringJobJson(throttles: Maybe<string[]?>.Some(null));
        var exception = Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<RecurringJob>(json));
        Assert.Equal("Value cannot be null. (Parameter 'throttles')", exception.Message);
    }

    /// <summary>
    /// Read should throw if it encounters a throttles property with a value
    /// that is an object that is not empty.
    /// </summary>
    [Fact]
    public void Read_Throttles_ThrowsIfANonEmptyObjectIsEncountered()
    {
        var json = RecurringJobFactory.RecurringJobJsonRaw(
            throttles: Maybe<string>.Some("""{"boom": "boom"}""")
        );
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<RecurringJob>(json)
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
        var json = RecurringJobFactory.RecurringJobJsonRaw(throttles: Maybe<string>.Some("{}"));
        var job = JsonSerializer.Deserialize<RecurringJob>(json);
        Assert.NotNull(job);
        Assert.Equivalent(Array.Empty<string>(), job.Throttles);
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
            var json = RecurringJobFactory.RecurringJobJsonRaw(unknown: Maybe<string>.Some(unknown));
            var job = JsonSerializer.Deserialize<RecurringJob>(json);
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
        string jid = "jid";
        int priority = 0;
        string queueName = "queue";
        int retries = 0;
        string state = "waiting";
        string[] tags = ["tag"];
        string[] throttles = ["throttle"];
        var json = RecurringJobFactory.RecurringJobJson(
            className: Maybe<string?>.Some(className),
            data: Maybe<string?>.Some(data),
            jid: Maybe<string?>.Some(jid),
            priority: Maybe<int?>.Some(priority),
            queueName: Maybe<string?>.Some(queueName),
            retries: Maybe<int?>.Some(retries),
            state: Maybe<string?>.Some(state),
            tags: Maybe<string[]?>.Some(tags),
            throttles: Maybe<string[]?>.Some(throttles)
        );

        var job = JsonSerializer.Deserialize<RecurringJob>(json);
        Assert.NotNull(job);
        Assert.Equal(className, job.ClassName);
        Assert.Equal(data, job.Data);
        Assert.Equal(jid, job.Jid);
        Assert.Equal(priority, job.Priority);
        Assert.Equal(queueName, job.QueueName);
        Assert.Equal(retries, job.Retries);
        Assert.Equal(state, job.State);
        Assert.Equal(tags, job.Tags);
        Assert.Equal(throttles, job.Throttles);
    }

    /// <summary>
    /// Write should be able to serialize a job with all properties set.
    /// </summary>
    [Fact]
    public void Write_CanSerializeJob()
    {
        var className = "klass";
        var count = 10;
        var data = "{}";
        var intervalSeconds = 60;
        var jid = "jid";
        var maximumBacklog = 10;
        var priority = 25;
        var queueName = "queueName";
        var retries = 6;
        var state = "waiting";
        var tags = new string[] { "tag" };
        var throttles = new string[] { "throttle" };

        var job = new RecurringJob(
            count: count,
            className: className,
            data: data,
            intervalSeconds: intervalSeconds,
            jid: jid,
            maximumBacklog: maximumBacklog,
            priority: priority,
            queueName: queueName,
            retries: retries,
            state: state,
            tags: tags,
            throttles: throttles
        );
        var json = JsonSerializer.Serialize(job);
        var expectedRecurringJobJson = RecurringJobFactory.RecurringJobJson(
            className: Maybe<string?>.Some(className),
            count: Maybe<int?>.Some(count),
            data: Maybe<string?>.Some(data),
            intervalSeconds: Maybe<int?>.Some(intervalSeconds),
            jid: Maybe<string?>.Some(jid),
            maximumBacklog: Maybe<int?>.Some(maximumBacklog),
            priority: Maybe<int?>.Some(priority),
            queueName: Maybe<string?>.Some(queueName),
            retries: Maybe<int?>.Some(retries),
            state: Maybe<string?>.Some(state),
            tags: Maybe<string[]?>.Some(tags),
            throttles: Maybe<string[]?>.Some(throttles)
        );
        Assert.Equal(expectedRecurringJobJson, json);
    }
}