namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Interface for a class that provides a list of queue names that a worker
/// should listen to.
/// </summary>
public interface IQueueNameProvider
{
    /// <summary>
    /// Gets a list of priority sorted queue names that a worker may consult for
    /// work until work is found.
    /// </summary>
    /// <returns>A list of queue names.</returns>
    public Task<List<string>> GetQueueNamesAsync();
}
