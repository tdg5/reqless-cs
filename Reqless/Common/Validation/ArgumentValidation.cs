namespace Reqless.Common.Validation;

/// <summary>
/// A helper class for functions that help validating arguments.
/// </summary>
public static class ArgumentValidation
{
    /// <summary>
    /// Throws a <see cref="ArgumentNullException"/> if the given value is null
    /// or throws a <see cref="ArgumentException"/> if the collection has length
    /// of zero.
    /// </summary>
    /// <param name="collection">The collection to validate.</param>
    /// <param name="paramName">The name of the parameter that is being
    /// validated.</param>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public static void ThrowIfNullOrEmpty<T>(
        IEnumerable<T> collection, string paramName)
    {
        ArgumentNullException.ThrowIfNull(collection, paramName);

        if (!collection.Any())
        {
            throw new ArgumentException("Value cannot be empty.", paramName);
        }
    }

    /// <summary>
    /// Checks an enumerable for values that are null and throws an <see
    /// cref="ArgumentException"/> if any are found.
    /// </summary>
    /// <param name="values">The array to validate.</param>
    /// <param name="paramName">The name of the parameter that is being
    /// validated.</param>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <exception cref="ArgumentNullException">If the values array itself is
    /// null.</exception>
    /// <exception cref="ArgumentException">If any of the values in the array
    /// are null.</exception>
    public static void ThrowIfAnyNull<T>(IEnumerable<T> values, string paramName)
        where T : class?
    {
        ArgumentNullException.ThrowIfNull(values, paramName);

        foreach (var value in values)
        {
            if (value is null)
            {
                throw new ArgumentException("Value cannot include null.", paramName);
            }
        }
    }

    /// <summary>
    /// Checks an array for values that are null, empty, or composed entirely of
    /// whitespace and throws an <see cref="ArgumentException"/> if any are found.
    /// </summary>
    /// <param name="values">The array to validate.</param>
    /// <param name="paramName">The name of the parameter that is being
    /// validated.</param>
    /// <exception cref="ArgumentNullException">If the values array itself is
    /// null.</exception>
    /// <exception cref="ArgumentException">If any of the values in the array
    /// are null, empty, or only whitespace.</exception>
    public static void ThrowIfAnyNullOrWhitespace(IEnumerable<string> values, string paramName)
    {
        ArgumentNullException.ThrowIfNull(values, paramName);

        foreach (var value in values)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    "Value cannot include null, empty string, or strings composed entirely of whitespace.",
                    paramName);
            }
        }
    }

    /// <summary>
    /// Check if a string is empty or composed entirely of whitespace, but only
    /// if the value is not null.
    /// cref="ArgumentException"/> if any are found.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The name of the parameter that is being
    /// validated.</param>
    /// <exception cref="ArgumentException">If the value is empty or composed
    /// entirely of whitespace.</exception>
    public static void ThrowIfNotNullAndEmptyOrWhitespace(string? value, string paramName)
    {
        if (value is null)
        {
            return;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
    }

    /// <summary>
    /// Check if an integer is less than zero and if so throw an <see
    /// cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The name of the parameter that is being
    /// validated.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the value is less than
    /// zero.</exception>
    public static void ThrowIfNegative(int value, string paramName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, "Value must be greater than or equal to zero.");
        }
    }

    /// <summary>
    /// Check if a long is less than zero and if so throw an <see
    /// cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The name of the parameter that is being
    /// validated.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the value is less than
    /// zero.</exception>
    public static void ThrowIfNegative(long value, string paramName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, "Value must be greater than or equal to zero.");
        }
    }

    /// <summary>
    /// Check if an integer is less than one and if so throw an <see
    /// cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The name of the parameter that is being
    /// validated.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the value is less than
    /// zero.</exception>
    public static void ThrowIfNotPositive(int value, string paramName)
    {
        if (value < 1)
        {
            throw new ArgumentOutOfRangeException(
                paramName, "Value must be greater than zero.");
        }
    }

    /// <summary>
    /// Check if an long is less than one and if so throw an <see
    /// cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The name of the parameter that is being
    /// validated.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the value is less than
    /// zero.</exception>
    public static void ThrowIfNotPositive(long value, string paramName)
    {
        if (value < 1)
        {
            throw new ArgumentOutOfRangeException(
                paramName, "Value must be greater than zero.");
        }
    }
}
