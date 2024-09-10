namespace Reqless.Framework;

/// <summary>
/// Provides access to the current <see cref="IJobContext"/>, if one is
/// available.
/// </summary>
public interface IJobContextAccessor
{
    /// <summary>
    /// Gets or sets the current <see cref="IJobContext"/>. Returns <see
    /// langword="null" /> if there is no active <see cref="IJobContext"/>.
    /// </summary>
    IJobContext? JobContext { get; set; }
}
