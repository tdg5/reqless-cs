using System.Text.Json;
using Reqless.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Models;

/// <summary>
/// Tests for the <see cref="JidsResult"/> class.
/// </summary>
public class JidsResultTest
{
    /// <summary>
    /// <see cref="JidsResult"/> should set the Jids and Total properties.
    /// </summary>
    [Fact]
    public void Constructor_SetsJidsAndTotal()
    {
        string[] jids = ["jid1", "jid2"];
        int total = 4;
        JidsResult result = new(jids, total);
        Assert.Equal(jids, result.Jids);
        Assert.Equal(total, result.Total);
    }

    /// <summary>
    /// <see cref="JidsResult"/> should throw if any of the jids are null,
    /// empty, or whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Jids_ThrowsIfAnyJidIsNullOrEmptyOrWhitespace()
    {
        int total = 4;
        Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespace(
            (invalidJid) => new JidsResult(
                ["jid1", "jid2", invalidJid!],
                total
            ),
            "jids"
        );
    }

    /// <summary>
    /// <see cref="JidsResult"/> should throw if jids is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsIfJidsIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new JidsResult(null!, 4),
            "jids"
        );
    }

    /// <summary>
    /// <see cref="JidsResult"/> can be deserialized from JSON.
    /// </summary>
    [Fact]
    public void CanBeDeserializedFromJson()
    {
        var total = 4;
        var jids = new[] { "jid1", "jid2" };
        var jidsJson = JsonSerializer.Serialize(jids);
        string json = $$"""{"total":{{total}},"jobs":{{jidsJson}}}""";
        JidsResult? result = JsonSerializer.Deserialize<JidsResult>(json);
        Assert.NotNull(result);
        Assert.Equal(jids, result.Jids);
        Assert.Equal(total, result.Total);
    }

    /// <summary>
    /// <see cref="JidsResult"/> can be serialized to JSON.
    /// </summary>
    [Fact]
    public void CanBeSerializedToJson()
    {
        var total = 4;
        var jids = new[] { "jid1", "jid2" };
        var subject = new JidsResult(jids, total);
        var json = JsonSerializer.Serialize(subject);
        var subjectFromJson = JsonSerializer.Deserialize<JidsResult>(json);
        Assert.NotNull(subjectFromJson);
        Assert.Equal(subject.Jids, subjectFromJson.Jids);
        Assert.Equal(subject.Total, subjectFromJson.Total);
    }
}