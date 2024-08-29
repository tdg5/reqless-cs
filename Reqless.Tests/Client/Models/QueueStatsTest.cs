using System.Text.Json;
using Reqless.Client.Models;

namespace Reqless.Tests.Client.Models;

/// <summary>
/// Unit tests for <see cref="QueueStats"/>.
/// </summary>
public class QueueStatsTest
{
    private static readonly int[] ExampleHistogramData = [1, 2, 3, 4, 5, 6, 7];

    /// <summary>
    /// <see cref="QueueStats"/> should be deserializable from JSON like that
    /// returned by Reqless server.
    /// </summary>
    [Fact]
    public void DeserializingFromJsonReturnsExpectedInstance()
    {
        var histogramJson = JsonSerializer.Serialize(ExampleHistogramData);
        var failed = 1;
        var failures = 10;
        var retries = 5;
        var count = 11;
        var mean = 23;
        var std = 42;
        var queueStatsJson = $$"""
            {
                "failed": {{failed}},
                "failures": {{failures}},
                "retries": {{retries}},
                "run": {
                    "count": {{count}},
                    "histogram": {{histogramJson}},
                    "mean": {{mean}},
                    "std": {{std}}
                },
                "wait": {
                    "count": {{count}},
                    "histogram": {{histogramJson}},
                    "mean": {{mean}},
                    "std": {{std}}
                }
            }
            """;
        var queueStats = JsonSerializer.Deserialize<QueueStats>(queueStatsJson);
        Assert.NotNull(queueStats);
        Assert.Equal(failed, queueStats.Failed);
        Assert.Equal(failures, queueStats.Failures);
        Assert.Equal(retries, queueStats.Retries);

        Assert.Equal(count, queueStats.Run.Count);
        Assert.Equal(ExampleHistogramData, queueStats.Run.Histogram);
        Assert.Equal(mean, queueStats.Run.Mean);
        Assert.Equal(std, queueStats.Run.StandardDeviation);

        Assert.Equal(count, queueStats.Wait.Count);
        Assert.Equal(ExampleHistogramData, queueStats.Wait.Histogram);
        Assert.Equal(mean, queueStats.Wait.Mean);
        Assert.Equal(std, queueStats.Wait.StandardDeviation);
    }

    /// <summary>
    /// <see cref="QueueStats"/> should be serializable to JSON like that
    /// returned by Reqless server.
    /// </summary>
    [Fact]
    public void SerializingToJsonReturnsJsonThatCanBeRoundTripped()
    {
        var expectedQueueStats = new QueueStats
        {
            Failed = 1,
            Failures = 2,
            Retries = 3,
            Run = new()
            {
                Count = 4,
                Histogram = ExampleHistogramData,
                Mean = 5,
                StandardDeviation = 6,
            },
            Wait = new()
            {
                Count = 7,
                Histogram = ExampleHistogramData,
                Mean = 8,
                StandardDeviation = 9,
            },
        };
        var queueStatsJson = JsonSerializer.Serialize(expectedQueueStats);
        var queueStats = JsonSerializer.Deserialize<QueueStats>(queueStatsJson);
        Assert.Equivalent(expectedQueueStats, queueStats);
    }
}