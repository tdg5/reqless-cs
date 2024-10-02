namespace Reqless.Worker;

/// <summary>
/// Interface for a cla a list of queue names that a worker should listen to.
/// </summary>
public interface IQueueNameProvider
{
    /// <summary>
    /// Gets a list of priority sorted queue names that a worker may consult for
    /// work until work is found.
    /// </summary>
    public Task<List<string>> GetQueueNamesAsync();
}
