namespace Reqless.Worker;

/// <summary>
/// Abstraction for generating names that workers can use when communicating
/// with Reqless.
/// </summary>
public interface IWorkerNameProvider
{
    /// <summary>
    /// Retrieve or generate a name that a worker can use when communicating
    /// with Reqless.
    /// </summary>
    public string GetWorkerName();
}
