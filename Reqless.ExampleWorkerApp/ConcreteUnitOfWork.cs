using Reqless.Framework;

namespace Reqless.ExampleWorkerApp;

public class ConcreteUnitOfWork : IUnitOfWork
{
    IThinger _thinger;

    public ConcreteUnitOfWork(
        IThinger thinger
    )
    {
        _thinger = thinger;
    }

    public Task PerformAsync()
    {
        _thinger.DoThing();
        return Task.CompletedTask;
    }
}