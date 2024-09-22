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

        var services = builder.Services;
        var reqlessWorkerSettings = new ReqlessWorkerSettings(
            queueIdentifiers: new List<string> { "example-queue" }.AsReadOnly(),
            workerCount: 2
        );
        services.AddSingleton<IThinger, Thinger>();
        services.AddReqlessWorkerServices(reqlessWorkerSettings);

        using IHost host = builder.Build();
        host.Run();
    }
}
