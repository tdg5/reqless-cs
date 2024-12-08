namespace Reqless.ExampleWorkerApp;

/// <summary>
/// Concrete implementation of <see cref="IThinger"/>. This is an arbitrary
/// class intended to demonstrate dependency injection.
/// </summary>
public class Thinger : IThinger
{
    /// <inheritdoc/>
    public void DoThing()
    {
        Console.WriteLine("THING DID!");
    }
}
