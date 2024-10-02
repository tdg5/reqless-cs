namespace Reqless.Worker;

/// <summary>
/// Provides an implementation of <see cref="IJobContextAccessor" /> based on
/// the current execution context.
/// </summary>
public class DefaultJobContextAccessor : ExecutionContextValueAccessor<IJobContext>, IJobContextAccessor
{
}