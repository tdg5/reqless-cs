namespace Reqless.Tests.TestHelpers;

/// <summary>
/// A collection of common test scenario assertions.
/// </summary>
public static class Scenario
{
    /// <summary>
    /// Asserts that the given action throws an <see cref="ArgumentNullException"/>
    /// for the expected parameter name.
    /// </summary>
    /// <param name="action">An action that should be used to cause an exception
    /// to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static void ThrowsArgumentNullException(
        Action action,
        string parameterName
    )
    {
        var nullException = Assert.Throws<ArgumentNullException>(
            () => action()
        );
        Assert.Equal(
            $"Value cannot be null. (Parameter '{parameterName}')",
            nullException.Message
        );
        Assert.Equal(parameterName, nullException.ParamName);
    }

    /// <summary>
    /// Asserts that the given action throws an <see
    /// cref="ArgumentNullException"/> for the expected parameter name.
    /// </summary>
    /// <param name="action">An action that should cause an exception to be
    /// thrown.</param>
    /// <param name="parameterName">The name of the parameter under
    /// test.</param>
    public static async Task ThrowsArgumentNullExceptionAsync(
        Func<Task> action,
        string parameterName
    )
    {
        var nullException = await Assert.ThrowsAsync<ArgumentNullException>(
            () => action()
        );
        Assert.Equal(
            $"Value cannot be null. (Parameter '{parameterName}')",
            nullException.Message
        );
        Assert.Equal(parameterName, nullException.ParamName);
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="ArgumentException"/>
    /// if the parameter is an empty string or composed entirely of whitespace.
    /// </summary>
    /// <param name="action">An action taking a nullable string value that
    /// should be used to cause an exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static void ThrowsWhenParameterIsEmptyOrWhitespace(
        Action<string> action,
        string parameterName
    )
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var argumentException = Assert.Throws<ArgumentException>(
                () => action(emptyString)
            );
            Assert.Equal(
                $"The value cannot be an empty string or composed entirely of whitespace. (Parameter '{parameterName}')",
                argumentException.Message
            );
            Assert.Equal(parameterName, argumentException.ParamName);
        }
    }

    /// <summary>
    /// Asserts that the given action throws an <see
    /// cref="ArgumentNullException"/> if the parameter is null, and an <see
    /// cref="ArgumentException"/> if the parameter is an empty string or
    /// composed entirely of whitespace.
    /// </summary>
    /// <param name="action">An action taking a nullable string value that
    /// should be used to cause an exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static async Task ThrowsWhenParameterIsEmptyOrWhitespaceAsync(
        Func<string, Task> action,
        string parameterName
    )
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var argumentException = await Assert.ThrowsAsync<ArgumentException>(
                () => action(emptyString)
            );
            Assert.Equal(
                $"The value cannot be an empty string or composed entirely of whitespace. (Parameter '{parameterName}')",
                argumentException.Message
            );
            Assert.Equal(parameterName, argumentException.ParamName);
        }
    }

    /// <summary>
    /// Asserts that the given action throws an <see
    /// cref="ArgumentNullException"/> if the parameter is null, and an <see
    /// cref="ArgumentException"/> if the parameter is an empty string or
    /// composed entirely of whitespace.
    /// </summary>
    /// <param name="action">An action taking a nullable string value that
    /// should be used to cause an exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static void ThrowsWhenParameterIsNullOrEmptyOrWhitespace(
        Action<string?> action,
        string parameterName
    )
    {
        ThrowsArgumentNullException(() => action(null!), parameterName);
        ThrowsWhenParameterIsEmptyOrWhitespace(action, parameterName);
    }

    /// <summary>
    /// Asserts that the given action throws an <see
    /// cref="ArgumentNullException"/> if the parameter is null, and an <see
    /// cref="ArgumentException"/> if the parameter is an empty string or
    /// composed entirely of whitespace.
    /// </summary>
    /// <param name="action">An action taking a nullable string value that
    /// should be used to cause an exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static async Task ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
        Func<string?, Task> action,
        string parameterName
    )
    {
        await ThrowsArgumentNullExceptionAsync(() => action(null!), parameterName);
        await ThrowsWhenParameterIsEmptyOrWhitespaceAsync(action, parameterName);
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="ArgumentException"/>
    /// when the given value is used as a member of the collection parameter.
    /// </summary>
    /// <param name="action">An action taking a nullable string value that
    /// should be used to cause the appropriate exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static void ThrowsWhenParameterItemIsNullOrEmptyOrWhitespace(
        Action<string?> action,
        string parameterName
    )
    {
        foreach (var invalidItem in TestConstants.EmptyStringsWithNull)
        {
            var argumentException = Assert.Throws<ArgumentException>(
                () => action(invalidItem)
            );
            Assert.Equal(
                $"Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter '{parameterName}')",
                argumentException.Message
            );
            Assert.Equal(parameterName, argumentException.ParamName);
        }
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="ArgumentException"/>
    /// when the given value is used as a member of the collection parameter.
    /// </summary>
    /// <param name="action">An action taking a nullable string value that
    /// should be used to cause the appropriate exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static async Task ThrowsWhenParameterItemIsNullOrEmptyOrWhitespaceAsync(
        Func<string?, Task> action,
        string parameterName
    )
    {
        foreach (var invalidItem in TestConstants.EmptyStringsWithNull)
        {
            var argumentException = await Assert.ThrowsAsync<ArgumentException>(
                () => action(invalidItem)
            );
            Assert.Equal(
                $"Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter '{parameterName}')",
                argumentException.Message
            );
            Assert.Equal(parameterName, argumentException.ParamName);
        }
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="ArgumentOutOfRangeException"/>
    /// when the given value is negative.
    /// </summary>
    /// <param name="action">An action taking a int value that should be used to
    /// cause the appropriate exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static void ThrowsWhenParameterIsNegative(
        Action<int> action,
        string parameterName
    )
    {
        var negativeValues = new int[] { -1000, -100, -10, -1 };
        foreach (var negativeValue in negativeValues)
        {
            var argumentException = Assert.Throws<ArgumentOutOfRangeException>(
                () => action(negativeValue)
            );
            Assert.Equal(
                $"Value must be greater than or equal to zero. (Parameter '{parameterName}')",
                argumentException.Message
            );
            Assert.Equal(parameterName, argumentException.ParamName);
        }
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="ArgumentOutOfRangeException"/>
    /// when the given value is negative.
    /// </summary>
    /// <param name="action">An action taking a int value that should be used to
    /// cause the appropriate exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static async Task ThrowsWhenParameterIsNegativeAsync(
        Func<int, Task> action,
        string parameterName
    )
    {
        var negativeValues = new int[] { -1000, -100, -10, -1 };
        foreach (var negativeValue in negativeValues)
        {
            var argumentException = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => action(negativeValue)
            );
            Assert.Equal(
                $"Value must be greater than or equal to zero. (Parameter '{parameterName}')",
                argumentException.Message
            );
            Assert.Equal(parameterName, argumentException.ParamName);
        }
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="ArgumentOutOfRangeException"/>
    /// when the given value is negative.
    /// </summary>
    /// <param name="action">An action taking a long value that should be used to
    /// cause the appropriate exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static void ThrowsWhenParameterIsNegative(
        Action<long> action,
        string parameterName
    )
    {
        var negativeValues = new long[] { -1000, -100, -10, -1 };
        foreach (var negativeValue in negativeValues)
        {
            var argumentException = Assert.Throws<ArgumentOutOfRangeException>(
                () => action(negativeValue)
            );
            Assert.Equal(
                $"Value must be greater than or equal to zero. (Parameter '{parameterName}')",
                argumentException.Message
            );
            Assert.Equal(parameterName, argumentException.ParamName);
        }
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="ArgumentOutOfRangeException"/>
    /// when the given value is negative.
    /// </summary>
    /// <param name="action">An action taking a long value that should be used to
    /// cause the appropriate exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static async Task ThrowsWhenParameterIsNegativeAsync(
        Func<long, Task> action,
        string parameterName
    )
    {
        var negativeValues = new long[] { -1000, -100, -10, -1 };
        foreach (var negativeValue in negativeValues)
        {
            var argumentException = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => action(negativeValue)
            );
            Assert.Equal(
                $"Value must be greater than or equal to zero. (Parameter '{parameterName}')",
                argumentException.Message
            );
            Assert.Equal(parameterName, argumentException.ParamName);
        }
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="ArgumentOutOfRangeException"/>
    /// when the given value is not positive.
    /// </summary>
    /// <param name="action">An action taking a int value that should be used to
    /// cause the appropriate exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static void ThrowsWhenParameterIsNotPositive(
        Action<int> action,
        string parameterName
    )
    {
        var invalidValues = new int[] { -1000, -100, -10, -1, 0 };
        foreach (var invalidValue in invalidValues)
        {
            var argumentException = Assert.Throws<ArgumentOutOfRangeException>(
                () => action(invalidValue)
            );
            Assert.Equal(
                $"Value must be greater than zero. (Parameter '{parameterName}')",
                argumentException.Message
            );
            Assert.Equal(parameterName, argumentException.ParamName);
        }
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="ArgumentOutOfRangeException"/>
    /// when the given value is not positive.
    /// </summary>
    /// <param name="action">An action taking a int value that should be used to
    /// cause the appropriate exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static async Task ThrowsWhenParameterIsNotPositiveAsync(
        Func<int, Task> action,
        string parameterName
    )
    {
        var invalidValues = new int[] { -1000, -100, -10, -1, 0 };
        foreach (var invalidValue in invalidValues)
        {
            var argumentException = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => action(invalidValue)
            );
            Assert.Equal(
                $"Value must be greater than zero. (Parameter '{parameterName}')",
                argumentException.Message
            );
            Assert.Equal(parameterName, argumentException.ParamName);
        }
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="ArgumentOutOfRangeException"/>
    /// when the given value is not positive.
    /// </summary>
    /// <param name="action">An action taking a long value that should be used to
    /// cause the appropriate exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static void ThrowsWhenParameterIsNotPositive(
        Action<long> action,
        string parameterName
    )
    {
        var invalidValues = new long[] { -1000, -100, -10, -1, 0 };
        foreach (var invalidValue in invalidValues)
        {
            var argumentException = Assert.Throws<ArgumentOutOfRangeException>(
                () => action(invalidValue)
            );
            Assert.Equal(
                $"Value must be greater than zero. (Parameter '{parameterName}')",
                argumentException.Message
            );
            Assert.Equal(parameterName, argumentException.ParamName);
        }
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="ArgumentOutOfRangeException"/>
    /// when the given value is not positive.
    /// </summary>
    /// <param name="action">An action taking a long value that should be used to
    /// cause the appropriate exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static async Task ThrowsWhenParameterIsNotPositiveAsync(
        Func<long, Task> action,
        string parameterName
    )
    {
        var invalidValues = new long[] { -1000, -100, -10, -1, 0 };
        foreach (var invalidValue in invalidValues)
        {
            var argumentException = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => action(invalidValue)
            );
            Assert.Equal(
                $"Value must be greater than zero. (Parameter '{parameterName}')",
                argumentException.Message
            );
            Assert.Equal(parameterName, argumentException.ParamName);
        }
    }
}