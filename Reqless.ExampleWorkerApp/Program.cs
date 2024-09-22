using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reqless.Worker;

namespace Reqless.ExampleWorkerApp;

/// <summary>
/// Main program entry point.
/// </summary>
public class Program
{
    /// <summary>
    /// Main program entry point.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddSingleton<IReqlessClientFactory>(
            new ReqlessClientFactory(() => new ReqlessClient())
        );
        builder.Services.AddSingleton<IWorkerFactory, GenericWorkerFactory<AsyncWorker>>();
        builder.Services.AddSingleton<IThinger, Thinger>();
        builder.Services.AddReqlessServices();

        var workerCount = 2;
        foreach (var index in Enumerable.Range(0, workerCount))
        {
            builder.Services.AddSingleton<IHostedService, WorkerService>();
        }

        using IHost host = builder.Build();
        host.Run();
    }
}
