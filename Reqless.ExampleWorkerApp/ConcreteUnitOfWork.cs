using Reqless.Framework;
using Reqless.Client;
using Reqless.Client.Models;
using System.Text.Json;

namespace Reqless.ExampleWorkerApp;

public class ConcreteUnitOfWork : IUnitOfWork
{
    IClient _client;

    IThinger _thinger;

    Job? _job = null;

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

    public async Task PerformAsync()
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
