using Reqless.Client.Models;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Tests.Common.Client.Models;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Tests for <see cref="DefaultJobContext"/>.
/// </summary>
public class DefaultJobContextTest
{
    /// <summary>
    /// <see cref="DefaultJobContext(Job)"/> throws when the given job is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGivenJobIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new DefaultJobContext(null!),
            "job"
        );
    }

    /// <summary>
    /// <see cref="DefaultJobContext(Job)"/> sets the job property with the
    /// expected job.
    /// </summary>
    [Fact]
    public void Constructor_SetsJobProperty()
    {
        var expectedJob = JobFactory.NewJob();
        var subject = new DefaultJobContext(expectedJob);
        Assert.Equal(expectedJob, subject.Job);
    }
}
