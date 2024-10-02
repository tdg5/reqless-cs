namespace Reqless.Worker;

/// <summary>
/// A generic value accessor that can hold a reference value for the duration of
/// a given execution context.
/// </summary>
/// <remarks>
/// This implementation is taken from
/// Microsoft.AspNetCore.Http.HttpContextAccessor, but made generic.
/// </remarks>
public class ExecutionContextValueAccessor<T> where T : class
{
    private static readonly AsyncLocal<ValueHolder> _valueCurrent = new();

    /// <inheritdoc/>
    public T? Value
    {
        get
        {
            return _valueCurrent.Value?.Value;
        }
        set
        {
            var holder = _valueCurrent.Value;
            if (holder != null)
            {
                // Clear completed value trapped in the AsyncLocal.
                holder.Value = null;
            }

            if (value != null)
            {
                // Use an object indirection to hold the IJobContext in the AsyncLocal,
                // so it can be cleared in all ExecutionContexts when its cleared.
                _valueCurrent.Value = new ValueHolder { Value = value };
            }
        }
    }

    private sealed class ValueHolder
    {
        public T? Value;
    }
}