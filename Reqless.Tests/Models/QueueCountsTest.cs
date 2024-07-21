using Reqless.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Models;

/// <summary>
/// Tests for the <see cref="QueueCounts"/> class.
/// </summary>
public class QueueCountsTest
{
    /// <summary>
    /// The constructor should throw if the given name is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsIfNameIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new QueueCounts
            {
                Depends = 0,
                Name = null!,
                Paused = false,
                Recurring = 0,
                Running = 0,
                Scheduled = 0,
                Stalled = 0,
                Throttled = 0,
                Waiting = 0
            }
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'Name')",
            exception.Message
        );
    }

    /// <summary>
    /// The constructor should throw if the given name value is empty or only
    /// whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsIfNameIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => new QueueCounts
                {
                    Depends = 0,
                    Name = emptyString,
                    Paused = false,
                    Recurring = 0,
                    Running = 0,
                    Scheduled = 0,
                    Stalled = 0,
                    Throttled = 0,
                    Waiting = 0
                }
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'Name')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// The constructor should initialize the name property.
    /// </summary>
    [Fact]
    public void Constructor_InitializesName()
    {
        var name = "test-queue";
        var queueCounts = new QueueCounts
        {
            Depends = 0,
            Name = name,
            Paused = false,
            Recurring = 0,
            Running = 0,
            Scheduled = 0,
            Stalled = 0,
            Throttled = 0,
            Waiting = 0
        };
        Assert.Equal(name, queueCounts.Name);
    }
}