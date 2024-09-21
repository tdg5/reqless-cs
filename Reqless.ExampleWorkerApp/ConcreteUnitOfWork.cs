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

    Job? _job = null;

    /// <summary>
    /// Create an instance of <see cref="ConcreteUnitOfWork"/>.
    /// </summary>
    public ConcreteUnitOfWork(
        IThinger thinger,
        IReqlessClientAccessor reqlessClientAccessor,
        Job? job = null
    )
    {
        _client = reqlessClientAccessor.Value
            ?? throw new InvalidOperationException("No client available.");
        _job = job;
        _thinger = thinger;
    }

    /// <inheritdoc/>
    public async Task PerformAsync(CancellationToken cancellationToken)
    {
        _thinger.DoThing();
        Console.WriteLine(
            JsonSerializer.Serialize(
                await _client.GetAllWorkerCountsAsync(),
                new JsonSerializerOptions { WriteIndented = true }
            )
        );
    }
}
