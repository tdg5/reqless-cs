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
        IServiceCollection services = builder.Services;

        services.AddSingleton<IReqlessClientFactory>(
            new DelegatingReqlessClientFactory(() => new ReqlessClient())
        );
        services.AddSingleton<IReqlessClient>(
            provider => provider.GetRequiredService<IReqlessClientFactory>().Create()
        );
        services.AddSingleton<IHostedService, Application>();

        using IHost host = builder.Build();
        host.Run();
    }
}
