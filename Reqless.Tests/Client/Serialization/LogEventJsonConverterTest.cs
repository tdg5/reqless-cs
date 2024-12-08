using Reqless.Client.Models;
using Reqless.Client.Serialization;
using System.Text.Json;

namespace Reqless.Tests.Client.Serialization;

/// <summary>
/// Tests for <see cref="LogEventJsonConverter"/>.
/// </summary>
public class LogEventJsonConverterTest
{
    /// <summary>
    /// Read should throw if the given JSON isn't an object.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonDoesNotStartWithObjectStart()
    {
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<LogEvent>("[]"));
        Assert.Equal(
            "Expected reader to begin with start of object.", exception.Message);
    }

    /// <summary>
    /// Read should throw if the given JSON object has a null "what" property.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonHasNullWhatProperty()
    {
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<LogEvent>(
                """{"when": 123, "what": null}"""));
        Assert.Equal(
            "Expected a string value for the 'what' property, got null.",
            exception.Message);
    }

    /// <summary>
    /// Read should throw if the given JSON object doesn't have a "what" property.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonDoesNotIncludeWhatProperty()
    {
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<LogEvent>("""{"when": 123}"""));
        Assert.Equal(
            "Expected 'what' property in JSON object, but none was found.",
            exception.Message);
    }

    /// <summary>
    /// Read should throw if the given JSON object does not include when property.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonDoesNotIncludeWhenProperty()
    {
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<LogEvent>("""{"what": "what"}"""));
        Assert.Equal(
            "Expected 'when' property in JSON object, but none was found.",
            exception.Message);
    }

    /// <summary>
    /// Read should throw if the given JSON object has a null when property.
    /// </summary>
    [Fact]
    public void Read_ThrowsIfJsonHasNullWhenProperty()
    {
        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<LogEvent>(
                """{"what": "what", "when": null}"""));
        Assert.Contains(
            "The JSON value could not be converted to Reqless.Client.Models.LogEvent",
            exception.Message);
    }

    /// <summary>
    /// Read gathers extra properties into the data dictionary.
    /// </summary>
    [Fact]
    public void Read_GathersExtraPropertiesIntoData()
    {
        var extra = "extra";
        var logEvent = JsonSerializer.Deserialize<LogEvent>(
            $$"""{"what": "what", "when": 123, "extra": "{{extra}}"}""");
        Assert.NotNull(logEvent);
        Assert.Equal("what", logEvent.What);
        Assert.Equal(123, logEvent.When);
        Assert.Equal(extra, logEvent.Data["extra"].GetString());
    }

    /// <summary>
    /// Write should serialize a LogEvent object to JSON.
    /// </summary>
    [Fact]
    public void Write_CanSerializeLogEvent()
    {
        var extra = "\"extra\"";
        var when = 123;
        var what = "what";
        var extraJsonElement = JsonDocument.Parse(extra).RootElement;
        var logEvent = new LogEvent(
            data: new Dictionary<string, JsonElement> { ["extra"] = extraJsonElement },
            what: what,
            when: when);
        var json = JsonSerializer.Serialize(logEvent);
        var jobEventFromJson = JsonSerializer.Deserialize<LogEvent>(json);
        var logEventFromJson = Assert.IsType<LogEvent>(jobEventFromJson);
        Assert.Equal(logEvent.What, logEventFromJson.What);
        Assert.Equal(logEvent.When, logEventFromJson.When);
        Assert.Equal(
            logEvent.Data["extra"].GetString(),
            logEventFromJson.Data["extra"].GetString());
    }
}
