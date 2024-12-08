namespace Reqless.Common.Validation;

/// <summary>
/// A helper class for functions that help validating operations.
/// </summary>
public static class OperationValidation
{
    /// <summary>
    /// Checks a subject for null and throws an <see cref="InvalidOperationException"/>
    /// if it is found.
    /// </summary>
    /// <param name="subject">The subject to check for nullness.</param>
    /// <param name="subjectName">The name to use to identify the subject in
    /// error messages.</param>
    /// <typeparam name="T">The type of the subject to check for nullness.</typeparam>
    /// <exception cref="InvalidOperationException">If the subject is
    /// null.</exception>
    public static void ThrowIfNull<T>(
        T subject, string subjectName)
        where T : class?
    {
        if (subject is null)
        {
            throw new InvalidOperationException(
                $"Value cannot be null. (Subject '{subjectName}')");
        }
    }

    /// <summary>
    /// Checks an <see cref="IEnumerable{T}"/> for elements that are null and throws
    /// an <see cref="InvalidOperationException"/> if any are found.
    /// </summary>
    /// <param name="subject">The <see cref="IEnumerable{T}"/> to validate.</param>
    /// <param name="subjectName">The name to use to identify the subject in error
    /// messages.</param>
    /// <typeparam name="T">The type of the elements in the <see
    /// cref="IEnumerable{T}"/>.</typeparam>
    /// <exception cref="InvalidOperationException">If the subject or any of its
    /// elements are null.</exception>
    public static void ThrowIfAnyNull<T>(
        IEnumerable<T> subject, string subjectName)
        where T : class?
    {
        ThrowIfNull(subject, subjectName);

        foreach (var value in subject)
        {
            if (value is null)
            {
                throw new InvalidOperationException(
                    $"Value cannot include null. (Subject '{subjectName}')");
            }
        }
    }

    /// <summary>
    /// Checks an enumerable for elements that are null or empty or composed
    /// entirely of whitespace and throws an <see
    /// cref="InvalidOperationException"/> if any are found.
    /// </summary>
    /// <param name="subject">The enumerable to validate.</param>
    /// <param name="subjectName">The name to use to identify the subject in error
    /// messages.</param>
    /// <exception cref="InvalidOperationException">If the subject or any of its
    /// elements are null, empty, or composed entirely of whitespace.</exception>
    public static void ThrowIfAnyNullOrWhitespace(
        IEnumerable<string> subject,
        string subjectName)
    {
        ThrowIfNull(subject, subjectName);

        foreach (var value in subject)
        {
            if (value is null)
            {
                throw new InvalidOperationException(
                    $"Value cannot include null. (Subject '{subjectName}')");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    "Value cannot include empty string or strings composed"
                    + $" entirely of whitespace. (Subject '{subjectName}')");
            }
        }
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if the response from the
    /// server is null.
    /// </summary>
    /// <param name="response">The response from the server.</param>
    /// <typeparam name="T">The type of the response from the server.</typeparam>
    /// <returns>The response from the server if it is not null.</returns>
    /// <exception cref="InvalidOperationException">If the server response is
    /// null.</exception>
    public static T ThrowIfServerResponseIsNull<T>(T? response)
        where T : class
    {
        if (response is not null)
        {
            return response;
        }

        throw new InvalidOperationException("Server returned unexpected null result.");
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if the response from the
    /// server is null.
    /// </summary>
    /// <param name="response">The response from the server.</param>
    /// <typeparam name="T">The type of the response from the server.</typeparam>
    /// <returns>The response from the server if it is not null.</returns>
    /// <exception cref="InvalidOperationException">If the server response is
    /// null.</exception>
    public static T ThrowIfServerResponseIsNull<T>(T? response)
    where T : struct
    {
        if (response is not null)
        {
            return response.Value;
        }

        throw new InvalidOperationException("Server returned unexpected null result.");
    }
}
