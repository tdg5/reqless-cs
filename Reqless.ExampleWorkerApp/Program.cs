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
        builder.Services.AddSingleton<IThinger, Thinger>();
        builder.Services.AddReqlessWorkerServices(new(
            queueIdentifiers: ["example-queue"],
            workerCount: 2
        ));

        using IHost host = builder.Build();
        host.Run();
    }
}
