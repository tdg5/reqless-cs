using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Reqless.Client;

namespace Reqless.ExampleClientApp;

public class Application : BackgroundService
{
    IHostApplicationLifetime _applicationLifetime;

    IReqlessClient _client;

    public Application(
        IHostApplicationLifetime applicationLifetime,
        IReqlessClient client
    )
    {
        _applicationLifetime = applicationLifetime;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine(
            JsonSerializer.Serialize(
                await _client.GetAllWorkerCountsAsync(),
                new JsonSerializerOptions { WriteIndented = true }
            )
        );
        _applicationLifetime.StopApplication();
    }
}
