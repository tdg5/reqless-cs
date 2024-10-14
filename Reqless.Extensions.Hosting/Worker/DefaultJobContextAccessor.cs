namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Provides an implementation of <see cref="IJobContextAccessor" />.
/// </summary>
public class DefaultJobContextAccessor : IJobContextAccessor
{
    private IJobContext? _value = null;

    /// <inheritdoc/>
    public IJobContext? Value
    {
        get => _value;
        set
        {
            if (_value is not null)
            {
                throw new InvalidOperationException(
                    "The job context has already been set."
                );
            }
            _value = value;
        }
    }
}
