using Moq;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Tests.Common.Client.Models;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Tests for <see cref="DefaultJobContextFactory"/>.
/// </summary>
public class DefaultJobContextFactoryTest
{
    /// <summary>
    /// <see cref="DefaultJobContextFactory.Create"/> throws when the given
    /// service provider is null.
    /// </summary>
    [Fact]
    public void Create_ThrowsWhenGivenServiceProviderIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new DefaultJobContextFactory().Create(null!, JobFactory.NewJob()),
            "serviceProvider");
    }

    /// <summary>
    /// <see cref="DefaultJobContextFactory.Create"/> throws when the given
    /// job is null.
    /// </summary>
    [Fact]
    public void Create_ThrowsWhenGivenJobIsNull()
    {
        DefaultJobContextFactory subject = new();
        Scenario.ThrowsWhenArgumentIsNull(
            () => subject.Create(Mock.Of<IServiceProvider>(), null!),
            "job");
    }

    /// <summary>
    /// <see cref="DefaultJobContextFactory.Create"/> returns a default job
    /// context.
    /// </summary>
    [Fact]
    public void Create_ReturnsDefaultJobContext()
    {
        var job = JobFactory.NewJob();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var subject = new DefaultJobContextFactory();
        var result = subject.Create(serviceProvider, job);
        Assert.NotNull(result);
        Assert.IsType<DefaultJobContext>(result);
    }
}
