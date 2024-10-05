using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reqless.Client;
using Reqless.Framework;

namespace Reqless.ExampleClientApp;

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
            new DelegatingReqlessClientFactory(() => new ReqlessClient())
        );
        builder.Services.AddReqlessServices();
        builder.Services.AddSingleton<IHostedService, Application>();

        using IHost host = builder.Build();
        host.Run();
    }
}
