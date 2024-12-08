using Reqless.Client;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Framework;
using System.Text.Json;

namespace Reqless.ExampleWorkerApp;

/// <summary>
/// A concrete <see cref="IUnitOfWork"/> that performs some arbitrary action.
/// </summary>
public class ConcreteUnitOfWork : IUnitOfWork
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
    };

    private readonly IReqlessClient _client;

    private readonly IThinger _thinger;

    private readonly IJobContextAccessor _jobContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcreteUnitOfWork"/>
    /// class.
    /// </summary>
    /// <param name="thinger">A thinger.</param>
    /// <param name="reqlessClient">A Reqless client.</param>
    /// <param name="jobContextAccessor">A job context accessor.</param>
    public ConcreteUnitOfWork(
        IThinger thinger,
        IReqlessClient reqlessClient,
        IJobContextAccessor jobContextAccessor)
    {
        _client = reqlessClient;
        _jobContextAccessor = jobContextAccessor;
        _thinger = thinger;
    }

    /// <inheritdoc/>
    public async Task PerformAsync(CancellationToken cancellationToken)
    {
        _thinger.DoThing();
        var jobContext = _jobContextAccessor.Value ??
            throw new InvalidOperationException("Job context is null.");
        Console.WriteLine(
            JsonSerializer.Serialize(jobContext.Job, JsonSerializerOptions));
        Console.WriteLine(
            JsonSerializer.Serialize(
                await _client.GetAllWorkerCountsAsync(), JsonSerializerOptions));
    }
}
