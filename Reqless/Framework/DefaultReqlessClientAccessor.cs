using Reqless.Client;

namespace Reqless.Framework;

/// <summary>
/// Provides an implementation of <see cref="IReqlessClientAccessor" /> based on
/// the current execution context.
/// </summary>
public class DefaultReqlessClientAccessor : ExecutionContextValueAccessor<IReqlessClient>, IReqlessClientAccessor
{
}
