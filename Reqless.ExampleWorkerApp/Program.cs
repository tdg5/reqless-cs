using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reqless.Client;
using Reqless.Framework;
using Reqless.Worker;

namespace Reqless.ExampleWorkerApp;

public class Program
{
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddSingleton<IReqlessClientFactory>(
            new ReqlessClientFactory(() => new ReqlessClient())
        );
        var workerCount = 2;
        builder.Services.AddReqlessServices();
        foreach (var index in Enumerable.Range(0, workerCount))
        {
            Console.WriteLine(index);
            builder.Services.AddSingleton<IHostedService, SerialWorker>();
        }
        builder.Services.AddSingleton<IThinger, Thinger>();

        using IHost host = builder.Build();
        host.Run();
    }
}
