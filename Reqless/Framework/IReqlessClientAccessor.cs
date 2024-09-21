using Reqless.Client;

namespace Reqless.Framework;

/// <summary>
/// Provides access to the current <see cref="IReqlessClient"/>, if one is available.
/// </summary>
public interface IReqlessClientAccessor
{
    /// <summary>
    /// Gets or sets the current <see cref="IReqlessClient"/>. Returns <see
    /// langword="null" /> if there is no active <see cref="IReqlessClient"/>.
    /// </summary>
    IReqlessClient? Value { get; set; }
}
