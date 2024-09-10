using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reqless.Framework;
using Reqless.Worker;

namespace Reqless.ExampleWorkerApp;

public class Program
{
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddReqlessServices();
        builder.Services.AddHostedService<SerialWorker>();
        builder.Services.AddSingleton<IThinger, Thinger>();

        using IHost host = builder.Build();
        host.Run();
    }
}
