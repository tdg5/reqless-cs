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
    public static void ThrowsWhenArgumentIsNull(
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
    public static async Task ThrowsWhenArgumentIsNullAsync(
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
    public static void ThrowsWhenArgumentIsEmptyOrWhitespace(
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
    public static async Task ThrowsWhenArgumentIsEmptyOrWhitespaceAsync(
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
    public static void ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
        Action<string?> action,
        string parameterName
    )
    {
        ThrowsWhenArgumentIsNull(() => action(null!), parameterName);
        ThrowsWhenArgumentIsEmptyOrWhitespace(action, parameterName);
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
    public static async Task ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
        Func<string?, Task> action,
        string parameterName
    )
    {
        await ThrowsWhenArgumentIsNullAsync(() => action(null!), parameterName);
        await ThrowsWhenArgumentIsEmptyOrWhitespaceAsync(action, parameterName);
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="ArgumentException"/>
    /// when the given value is used as a member of the collection parameter.
    /// </summary>
    /// <param name="action">An action taking a nullable string value that
    /// should be used to cause the appropriate exception to be thrown.</param>
    /// <param name="parameterName">The name of the parameter under test.</param>
    public static void ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespace(
        Action<string?> action,
        string parameterName
    )
    {
        foreach (var invalidElement in TestConstants.EmptyStringsWithNull)
        {
            var argumentException = Assert.Throws<ArgumentException>(
                () => action(invalidElement)
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
    public static async Task ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespaceAsync(
        Func<string?, Task> action,
        string parameterName
    )
    {
        foreach (var invalidElement in TestConstants.EmptyStringsWithNull)
        {
            var argumentException = await Assert.ThrowsAsync<ArgumentException>(
                () => action(invalidElement)
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
    public static void ThrowsWhenArgumentIsNegative(
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
    public static async Task ThrowsWhenArgumentIsNegativeAsync(
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
    public static void ThrowsWhenArgumentIsNegative(
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
    public static async Task ThrowsWhenArgumentIsNegativeAsync(
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
    public static void ThrowsWhenArgumentIsNotPositive(
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
    public static async Task ThrowsWhenArgumentIsNotPositiveAsync(
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
    public static void ThrowsWhenArgumentIsNotPositive(
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
    public static async Task ThrowsWhenArgumentIsNotPositiveAsync(
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

    /// <summary>
    /// Asserts that the given action throws an <see cref="InvalidOperationException"/>
    /// for the expected subject name.
    /// </summary>
    /// <param name="action">An action that should be used to cause an exception
    /// to be thrown.</param>
    /// <param name="subjectName">The name of the subject under test.</param>
    public static void ThrowsWhenOperationEncountersNull(
        Action action,
        string subjectName
    )
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => action()
        );
        Assert.Equal(
            $"Value cannot be null. (Subject '{subjectName}')",
            exception.Message
        );
    }

    /// <summary>
    /// Asserts that the given action throws an <see
    /// cref="InvalidOperationException"/> for the expected subject name when
    /// that subject includes a null value.
    /// </summary>
    /// <param name="action">An action that should be used to cause an exception
    /// to be thrown.</param>
    /// <param name="subjectName">The name of the subject under test.</param>
    public static void ThrowsWhenOperationEncountersNullElement(
        Action action,
        string subjectName
    )
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => action()
        );
        Assert.Equal(
            $"Value cannot include null. (Subject '{subjectName}')",
            exception.Message
        );
    }

    /// <summary>
    /// Asserts that the given async action throws an <see
    /// cref="InvalidOperationException"/> for the expected subject name when
    /// that subject includes a null value.
    /// </summary>
    /// <param name="action">An action that should be used to cause an exception
    /// to be thrown.</param>
    /// <param name="subjectName">The name of the subject under test.</param>
    public static async Task ThrowsWhenOperationEncountersElementThatIsNullAsync(
        Func<Task> action,
        string subjectName
    )
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(action);
        Assert.Equal(
            $"Value cannot include null. (Subject '{subjectName}')",
            exception.Message
        );
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="InvalidOperationException"/>
    /// for the expected subject name when that subject includes a value that is null,
    /// an empty string, or composed entirely of whitespace.
    /// </summary>
    /// <param name="action">An action that should be used to cause an exception
    /// to be thrown.</param>
    /// <param name="subjectName">The name of the subject under test.</param>
    public static async Task ThrowsWhenOperationEncountersElementThatIsNullOrEmptyOrWhitespaceAsync(
        Func<string, Task> action,
        string subjectName
    )
    {
        var nullException = await Assert.ThrowsAsync<InvalidOperationException>(
            () => action(null!)
        );
        Assert.Equal(
            $"Value cannot include null. (Subject '{subjectName}')",
            nullException.Message
        );

        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var emptyException = await Assert.ThrowsAsync<InvalidOperationException>(
                () => action(emptyString)
            );
            Assert.Equal(
                $"Value cannot include empty string or strings composed entirely of whitespace. (Subject '{subjectName}')",
                emptyException.Message
            );
        }
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="InvalidOperationException"/>
    /// for the expected subject name when the server returns a simulated null response.
    /// </summary>
    /// <param name="action">An action that should be used to cause an exception
    /// to be thrown.</param>
    public static void ThrowsWhenServerRespondsWithNull(Action action)
    {
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// Asserts that the given action throws an <see cref="InvalidOperationException"/>
    /// for the expected subject name when the server returns a simulated null response.
    /// </summary>
    /// <param name="action">An action that should be used to cause an exception
    /// to be thrown.</param>
    public static async Task ThrowsWhenServerRespondsWithNullAsync(Func<Task> action)
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(action);
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }
}