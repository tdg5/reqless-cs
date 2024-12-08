using Reqless.Extensions.Hosting.Worker;
using Reqless.Tests.Common.Client.Models;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Test class for <see cref="DefaultJobContextAccessor"/>.
/// </summary>
public class DefaultJobContextAccessorTest
{
    /// <summary>
    /// <see cref="DefaultJobContextAccessor.Value"/> should throw when the
    /// value is already set.
    /// </summary>
    [Fact]
    public void Value_Set_ThrowsWhenValueIsAlreadySet()
    {
        DefaultJobContextAccessor subject = new()
        {
            Value = new DefaultJobContext(JobFactory.NewJob()),
        };
        var exception = Assert.Throws<InvalidOperationException>(
            () => subject.Value = new DefaultJobContext(JobFactory.NewJob()));
        Assert.Equal(
            "The job context has already been set.", exception.Message);
    }

    /// <summary>
    /// <see cref="DefaultJobContextAccessor.Value"/> should return the value
    /// held by value.
    /// </summary>
    [Fact]
    public void Value_Get_ThrowsWhenValueIsAlreadySet()
    {
        DefaultJobContextAccessor subject = new();
        Assert.Null(subject.Value);
        var expectedJobContext = new DefaultJobContext(JobFactory.NewJob());
        subject.Value = expectedJobContext;
        Assert.Equal(expectedJobContext, subject.Value);
    }
}
