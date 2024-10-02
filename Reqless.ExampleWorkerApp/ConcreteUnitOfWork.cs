using Reqless.Framework;
using Reqless.Client;
using Reqless.Client.Models;
using System.Text.Json;

namespace Reqless.ExampleWorkerApp;

/// <summary>
/// A concrete <see cref="IUnitOfWork"/> that performs some arbitrary action.
/// </summary>
public class ConcreteUnitOfWork : IUnitOfWork
{
    IReqlessClient _client;

    IThinger _thinger;

    IJobContextAccessor _jobContextAccessor;

    /// <summary>
    /// Create an instance of <see cref="ConcreteUnitOfWork"/>.
    /// </summary>
    public ConcreteUnitOfWork(
        IThinger thinger,
        IReqlessClient reqlessClient,
        IJobContextAccessor jobContextAccessor
    )
    {
        _client = reqlessClient;
        _jobContextAccessor = jobContextAccessor;
        _thinger = thinger;
    }

    /// <inheritdoc/>
    public async Task PerformAsync(CancellationToken cancellationToken)
    {
        _thinger.DoThing();
        var jobContext = _jobContextAccessor.Value ??
            throw new InvalidOperationException("Job context is null.");
        Console.WriteLine(
            JsonSerializer.Serialize(
                jobContext.Job,
                new JsonSerializerOptions { WriteIndented = true }
            )
        );
        Console.WriteLine(
            JsonSerializer.Serialize(
                await _client.GetAllWorkerCountsAsync(),
                new JsonSerializerOptions { WriteIndented = true }
            )
        );
    }
}
