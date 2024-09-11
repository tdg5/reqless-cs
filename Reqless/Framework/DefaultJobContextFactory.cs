using Microsoft.Extensions.DependencyInjection;
using Reqless.Client.Models;

namespace Reqless.Framework;

/// <summary>
/// Default concrete implementation of <see cref="IJobContextFactory"/>.
/// </summary>
public class DefaultJobContextFactory : IJobContextFactory
{
    private readonly IJobContextAccessor? _jobContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultJobContextFactory"/>
    /// class.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/>
    /// instance that should be used to resolve services.</param>
    public DefaultJobContextFactory(IServiceProvider serviceProvider)
    {
        _jobContextAccessor = serviceProvider.GetService<IJobContextAccessor>();
    }

    /// <inheritdoc />
    public IJobContext Create(Job job)
    {
        DefaultJobContext jobContext = new(job);
        if (_jobContextAccessor != null)
        {
            _jobContextAccessor.Value = jobContext;
        }
        return jobContext;
    }
}
