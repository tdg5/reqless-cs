namespace Reqless.Common.Utilities;

/// <summary>
/// Static helper class for consisely creating instances of Maybe.
/// </summary>
public static class Maybe
{
    /// <summary>
    /// Creates a Maybe of type T with a value present (i.e., a Some of type T).
    /// </summary>
    /// <param name="value">The value to wrap in a <see
    /// cref="Maybe{T}"/>.</param>
    /// <typeparam name="T">The type of the value to wrap.</typeparam>
    /// <returns>An instance of <see cref="Maybe{T}.Some"/> wrapping the given
    /// value.</returns>
    public static Maybe<T> Some<T>(T value) => Maybe<T>.Some(value);
}
