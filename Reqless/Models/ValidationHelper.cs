namespace Reqless.Models;

/// <summary>
/// A helper class for functions that help validating arguments.
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Checks an array for values that are null and throws an <see
    /// cref="ArgumentException"/> if any are found.
    /// </summary>
    /// <param name="values">The array to validate.</param>
    /// <param name="paramName">The name of the parameter that is being
    /// validated.</param>
    /// <exception cref="ArgumentNullException">If the values array itself is
    /// null.</exception>
    /// <exception cref="ArgumentException">If any of the values in the array
    /// are null.</exception>
    public static void ThrowIfAnyNull<T>(T[] values, string paramName) where T : class?
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
    public static void ThrowIfAnyNullOrWhitespace(string[] values, string paramName)
    {
        ArgumentNullException.ThrowIfNull(values, paramName);

        foreach (var value in values)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    "Value cannot include null, empty string, or strings composed entirely of whitespace.",
                    paramName
                );
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
}