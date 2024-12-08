namespace Reqless.Common.Utilities;

/// <summary>
/// Represents a value that may or may not be present. Maybe represents a
/// discriminated union type that offers two constructors: Some and None. When
/// a value of type T exists, it is encapsulated within Maybe using the Some
/// constructor. If there is no value present, the None constructor is used to
/// indicate absence.
/// </summary>
/// <typeparam name="T">The type of the value that may or may not be present.</typeparam>
public abstract class Maybe<T>
{
    private static readonly Maybe<T> NoneSingleton = new Options.None();

    /// <summary>
    /// Gets a value tha represents the absence of a value of type T. None
    /// represents the absence of a value, and is used to indicate that no value
    /// is present.
    /// </summary>
    /// <returns>A <see cref="Maybe{T}"/> representing the absence of a value of
    /// type T.</returns>
    public static Maybe<T> None { get; } = NoneSingleton;

    /// <summary>
    /// Gets a value indicating whether the maybe encapsulates a value (i.e., a
    /// Some of type T), or false if no value is present (i.e., a None of type
    /// T).
    /// </summary>
    public abstract bool HasValue { get; }

    /// <summary>
    /// Gets a value indicating whether the maybe encapsulates no value (i.e., a
    /// None of type T), or false if a value is present (i.e., a Some of type
    /// T).
    /// </summary>
    public bool IsEmpty => !HasValue;

    /// <summary>
    /// Casts a value of type T to a Maybe of type T. Some represents a value
    /// that exists or is defined, though that value could be null.
    /// </summary>
    /// <param name="value">The defined value to convert to a Maybe.</param>
    /// <returns>A Maybe encapsulating the value.</returns>
    public static explicit operator Maybe<T>(T value) => Some(value);

    /// <summary>
    /// Encapsulate a value of type T as a Maybe of type T. Some represents a
    /// value that exists or is defined, though that value could be null.
    /// </summary>
    /// <param name="value">The defined value to convert to a Maybe.</param>
    /// <returns>A Maybe encapsulating the value.</returns>
    public static Maybe<T> Some(T value) => new Options.Some(value);

    /// <summary>
    /// Returns the value of the Maybe if it is present (i.e., a Some of type
    /// T), or the default value provided if the Maybe is not present (i.e., a
    /// None of type T).
    /// </summary>
    /// <param name="defaultValue">The default value to return if the Maybe is
    /// not present (i.e., a None of type T).</param>
    /// <returns>The value of the Maybe if it is present or the given default value.</returns>
    public abstract T GetOrDefault(T defaultValue);

    /// <summary>
    /// Applies a function to the value of the Maybe if it is present (i.e., a
    /// Some of type T), or returns the result of another function if the Maybe
    /// is not present (i.e., a None of type T).
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the functions.</typeparam>
    /// <param name="someFunc">The function to apply to the value of the Maybe if
    /// it is present (i.e., a Some of type T).</param>
    /// <param name="noneFunc">The function to apply if the Maybe is not present
    /// (i.e., a None of type T).</param>
    /// <returns>The result of applying the appropriate function to the Maybe.</returns>
    public abstract TResult Match<TResult>(
        Func<T, TResult> someFunc, Func<TResult> noneFunc);

    /// <summary>
    /// Maps the value of the Maybe to a new value of type R using the provided
    /// function. If the Maybe is not present (i.e., a None of type T), the
    /// function is not applied and a Maybe of type R is returned with no value
    /// present (i.e., a None of type R).
    /// </summary>
    /// <typeparam name="TResult">The type of the new value to map to.</typeparam>
    /// <param name="mapFunc">The function to apply to the value of the Maybe if
    /// it is present (i.e., a Some of type T).</param>
    /// <returns>A Maybe of type R with the new value (i.e., a Some of type R),
    /// if the Maybe is present (i.e., a Some of type T), or a Maybe of type R
    /// with no value present (i.e., a None of type R) if the Maybe has no value
    /// (i.e., a None of type T).</returns>
    public Maybe<TResult> Map<TResult>(Func<T, TResult> mapFunc)
    {
        return Match(
            v => Maybe<TResult>.Some(mapFunc(v)),
            () => Maybe<TResult>.None);
    }

    private static class Options
    {
        public sealed class Some(T value) : Maybe<T>
        {
            /// <inheritdoc/>
            public override bool HasValue => true;

            private T Value => value;

            public override T GetOrDefault(T defaultValue) => Value;

            public override TResult Match<TResult>(
                Func<T, TResult> someFunc, Func<TResult> noneFunc)
            {
                return someFunc(Value);
            }
        }

        public sealed class None : Maybe<T>
        {
            public override bool HasValue => false;

            public override T GetOrDefault(T defaultValue) => defaultValue;

            public override TResult Match<TResult>(
                Func<T, TResult> someFunc, Func<TResult> noneFunc) => noneFunc();
        }
    }
}
