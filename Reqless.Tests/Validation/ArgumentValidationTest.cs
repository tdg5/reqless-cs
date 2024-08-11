using Reqless.Validation;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Validation;

/// <summary>
/// Tests for the <see cref="ArgumentValidation"/> class.
/// </summary>
public class ArgumentValidationTest
{
    /// <summary>
    /// <see cref="ArgumentValidation.ThrowIfAnyNull"/> should throw if the
    /// collection is null.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNull_ThrowsIfCollectionIsNull()
    {
        var paramName = "param";
        var exception = Assert.Throws<ArgumentNullException>(
            () => ArgumentValidation.ThrowIfAnyNull<string>(null!, paramName)
        );
        Assert.Equal(
            $"Value cannot be null. (Parameter '{paramName}')",
            exception.Message
        );
        Assert.Equal(paramName, exception.ParamName);
    }

    /// <summary>
    /// <see cref="ArgumentValidation.ThrowIfAnyNull"/> should throw if any
    /// values are null.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNull_ThrowsIfAnyValuesAreNull()
    {
        var values = new string?[] { "a", null, "c" };
        var paramName = "values";

        var exception = Assert.Throws<ArgumentException>(
            () => ArgumentValidation.ThrowIfAnyNull(values, paramName)
        );
        Assert.Equal(
            $"Value cannot include null. (Parameter '{paramName}')",
            exception.Message
        );
        Assert.Equal(paramName, exception.ParamName);
    }

    /// <summary>
    /// <see cref="ArgumentValidation.ThrowIfAnyNull"/> should not throw if none
    /// of the values are null.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNull_NotThrowWhenNoValuesAreNull()
    {
        ArgumentValidation.ThrowIfAnyNull(["a", "b", "c"], "param");
    }

    /// <summary>
    /// <see cref="ArgumentValidation.ThrowIfAnyNullOrWhitespace"/> should throw
    /// if the collection is null.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNullOrWhitespace_ThrowsIfCollectionIsNull()
    {
        var paramName = "param";
        var exception = Assert.Throws<ArgumentNullException>(
            () => ArgumentValidation.ThrowIfAnyNullOrWhitespace(null!, paramName)
        );
        Assert.Equal(
            $"Value cannot be null. (Parameter '{paramName}')",
            exception.Message
        );
        Assert.Equal(paramName, exception.ParamName);
    }

    /// <summary>
    /// <see cref="ArgumentValidation.ThrowIfAnyNullOrWhitespace"/> should throw if any
    /// values are null, empty, or just whitespace.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNullOrWhitespace_ThrowsIfAnyValuesAreNullEmptyOrWhitespace()
    {
        var paramName = "values";
        Scenario.ThrowsWhenParameterItemIsNullOrEmptyOrWhitespace(
            (invalidValue) => ArgumentValidation.ThrowIfAnyNullOrWhitespace(
                ["a", invalidValue!, "c"],
                paramName
            ),
            paramName
        );
    }

    /// <summary>
    /// <see cref="ArgumentValidation.ThrowIfAnyNullOrWhitespace"/> should not
    /// throw if all the values are valid.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNullOrWhitespace_NotThrowWhenNoValuesAreNull()
    {
        ArgumentValidation.ThrowIfAnyNullOrWhitespace(["a", "b", "c"], "param");
    }

    /// <summary>
    /// <see cref="ArgumentValidation.ThrowIfNotNullAndEmptyOrWhitespace"/> should
    /// not throw if the value is null.
    /// </summary>
    [Fact]
    public void ThrowIfNotNullAndEmptyOrWhitespace_DoesNotThrowForNull()
    {
        ArgumentValidation.ThrowIfNotNullAndEmptyOrWhitespace(null, "param");
    }

    /// <summary>
    /// <see cref="ArgumentValidation.ThrowIfNotNullAndEmptyOrWhitespace"/> should
    /// throw the value is empty or whitespace.
    /// </summary>
    [Fact]
    public void ThrowIfNotNullAndEmptyOrWhitespace_ThrowsForEmptyOrWhitespace()
    {
        var paramName = "param";
        Scenario.ThrowsWhenParameterIsEmptyOrWhitespace(
            (invalidValue) => ArgumentValidation.ThrowIfNotNullAndEmptyOrWhitespace(
                invalidValue,
                paramName
            ),
            paramName
        );
    }

    /// <summary>
    /// <see cref="ArgumentValidation.ThrowIfAnyNullOrWhitespace"/> should not
    /// throw if all the values are valid.
    /// </summary>
    [Fact]
    public void ThrowIfNotNullAndEmptyOrWhitespace_DoesNotThrowForValidValue()
    {
        ArgumentValidation.ThrowIfNotNullAndEmptyOrWhitespace("valid", "param");
    }

    /// <summary>
    /// <see cref="ArgumentValidation.ThrowIfNegative(int, string)"/> should
    /// throw if a negative value is provided.
    /// </summary>
    [Fact]
    public void ThrowIfNegative_Int_ThrowsIfValueIsNegative()
    {
        var paramName = "param";
        ArgumentValidation.ThrowIfNegative(0, paramName);
        Scenario.ThrowsWhenParameterIsNegative(
            (int invalidValue) => ArgumentValidation.ThrowIfNegative(
                invalidValue,
                paramName
            ),
            paramName
        );
    }

    /// <summary>
    /// <see cref="ArgumentValidation.ThrowIfNegative(long, string)"/> should
    /// throw if a negative value is provided.
    /// </summary>
    [Fact]
    public void ThrowIfNegative_Long_ThrowsIfValueIsNegative()
    {
        var paramName = "param";
        ArgumentValidation.ThrowIfNegative(0L, paramName);
        Scenario.ThrowsWhenParameterIsNegative(
            (long invalidValue) => ArgumentValidation.ThrowIfNegative(
                invalidValue,
                paramName
            ),
            paramName
        );
    }
}