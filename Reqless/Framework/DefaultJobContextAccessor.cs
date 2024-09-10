namespace Reqless.Framework;

/// <summary>
/// Provides an implementation of <see cref="IJobContextAccessor" /> based on
/// the current execution context.
/// </summary>
/// <remarks>
/// This implementation is taken from
/// Microsoft.AspNetCore.Http.HttpContextAccessor.
/// </remarks>
public class DefaultJobContextAccessor : IJobContextAccessor
{
    private static readonly AsyncLocal<JobContextHolder> _jobContextCurrent = new();

    /// <inheritdoc/>
    public IJobContext? JobContext
    {
        get
        {
            return _jobContextCurrent.Value?.Context;
        }
        set
        {
            var holder = _jobContextCurrent.Value;
            if (holder != null)
            {
                // Clear completed JobContext trapped in the AsyncLocal.
                holder.Context = null;
            }

            if (value != null)
            {
                // Use an object indirection to hold the IJobContext in the AsyncLocal,
                // so it can be cleared in all ExecutionContexts when its cleared.
                _jobContextCurrent.Value = new JobContextHolder { Context = value };
            }
        }
    }

    private sealed class JobContextHolder
    {
        public IJobContext? Context;
    }
}