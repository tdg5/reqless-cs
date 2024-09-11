using Reqless.Client;

namespace Reqless.Framework;

/// <summary>
/// Provides access to the current <see cref="IClient"/>, if one is available.
/// </summary>
public interface IReqlessClientAccessor
{
    /// <summary>
    /// Gets or sets the current <see cref="IClient"/>. Returns <see
    /// langword="null" /> if there is no active <see cref="IClient"/>.
    /// </summary>
    IClient? Value { get; set; }
}
