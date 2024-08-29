using System.Text.Json;
using Reqless.Client.Models;

namespace Reqless.Tests.Client.Models;

/// <summary>
/// Unit tests for <see cref="QueueStats"/>.
/// </summary>
public class QueueStateStatsTest
{
    private static readonly int[] ExampleHistogramData = [1, 2, 3, 4, 5, 6, 7];

    /// <summary>
    /// <see cref="QueueStateStats"/> should be deserializable from JSON like that
    /// returned by Reqless server.
    /// </summary>
    [Fact]
    public void DeserializingFromJsonReturnsExpectedInstance()
    {
        var histogramJson = JsonSerializer.Serialize(ExampleHistogramData);
        var count = 11;
        var mean = 23;
        var std = 42;
        var queueStateStatsJson = $$"""
            {
                "count": {{count}},
                "histogram": {{histogramJson}},
                "mean": {{mean}},
                "std": {{std}}
            }
            """;
        var queueStateStats = (
            JsonSerializer.Deserialize<QueueStateStats>(queueStateStatsJson)
        );
        Assert.NotNull(queueStateStats);
        Assert.Equal(count, queueStateStats.Count);
        Assert.Equal(ExampleHistogramData, queueStateStats.Histogram);
        Assert.Equal(mean, queueStateStats.Mean);
        Assert.Equal(std, queueStateStats.StandardDeviation);
    }

    /// <summary>
    /// <see cref="QueueStateStats"/> should be serializable to JSON like that
    /// returned by Reqless server.
    /// </summary>
    [Fact]
    public void SerializingToJsonReturnsJsonThatCanBeRoundTripped()
    {
        var expectedQueueStateStats = new QueueStateStats
        {
            Count = 1,
            Histogram = ExampleHistogramData,
            Mean = 2,
            StandardDeviation = 3,
        };
        var queueStateStatsJson = JsonSerializer.Serialize(expectedQueueStateStats);
        var queueStateStats = (
            JsonSerializer.Deserialize<QueueStateStats>(queueStateStatsJson)
        );
        Assert.Equivalent(expectedQueueStateStats, queueStateStats);
    }
}