using Microsoft.Extensions.Hosting;
using Reqless.Client;
using System.Text.Json;

namespace Reqless.ExampleClientApp;

/// <summary>
/// Example background service application.
/// </summary>
public class Application : BackgroundService
{
    IHostApplicationLifetime _applicationLifetime;

    IReqlessClient _client;

    /// <summary>
    /// Create an instance of <see cref="Application"/>.
    /// </summary>
    /// <param name="applicationLifetime">The <see cref="IHostApplicationLifetime"/>
    /// instance that should be used to stop the application.</param>
    /// <param name="client">The <see cref="IReqlessClient"/> instance that should
    /// be used to interact with the Reqless server.</param>
    public Application(
        IHostApplicationLifetime applicationLifetime,
        IReqlessClient client
    )
    {
        _applicationLifetime = applicationLifetime;
        _client = client;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string jid = await _client.PutJobAsync(
            className: "Reqless.ExampleWorkerApp.ConcreteUnitOfWork",
            data: "{}",
            queueName: "example-queue",
            workerName: "example-worker"
        );
        Console.WriteLine(jid);
        Console.WriteLine(
            JsonSerializer.Serialize(
                await _client.GetAllWorkerCountsAsync(),
                new JsonSerializerOptions { WriteIndented = true }
            )
        );
        _applicationLifetime.StopApplication();
    }
}
