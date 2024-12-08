using Reqless.Common.Validation;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Default implementation of <see cref="IWorkerNameProvider"/> that generates
/// worker names with a constant prefix followed by a monotonically increasing
/// number.
/// </summary>
public class DefaultWorkerNameProvider : IWorkerNameProvider
{
    private readonly string _prefix;

    private int _counter = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultWorkerNameProvider"/>
    /// class.
    /// </summary>
    /// <param name="prefix">The prefix that should be used for all worker
    /// names.</param>
    public DefaultWorkerNameProvider(string? prefix = null)
    {
        ArgumentValidation.ThrowIfNotNullAndEmptyOrWhitespace(
            prefix,
            nameof(prefix));

        _prefix = prefix ?? "ReqlessWorker";
    }

    /// <inheritdoc/>
    public string GetWorkerName()
    {
        var index = Interlocked.Increment(ref _counter);
        return $"{_prefix}-{index}";
    }
}
