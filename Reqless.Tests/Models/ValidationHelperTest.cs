using Reqless.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Models;

/// <summary>
/// Tests for the <see cref="ValidationHelper"/> class.
/// </summary>
public class ValidationHelperTest
{
    /// <summary>
    /// <see cref="ValidationHelper.ThrowIfAnyNull"/> should throw if the
    /// collection is null.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNull_ThrowsIfCollectionIsNull()
    {
        var paramName = "param";
        var exception = Assert.Throws<ArgumentNullException>(
            () => ValidationHelper.ThrowIfAnyNull<string>(null!, paramName)
        );
        Assert.Equal(
            $"Value cannot be null. (Parameter '{paramName}')",
            exception.Message
        );
        Assert.Equal(paramName, exception.ParamName);
    }

    /// <summary>
    /// <see cref="ValidationHelper.ThrowIfAnyNull"/> should throw if any
    /// values are null.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNull_ThrowsIfAnyValuesAreNull()
    {
        var values = new string?[] { "a", null, "c" };
        var paramName = "values";

        var exception = Assert.Throws<ArgumentException>(
            () => ValidationHelper.ThrowIfAnyNull(values, paramName)
        );
        Assert.Equal(
            $"Value cannot include null. (Parameter '{paramName}')",
            exception.Message
        );
        Assert.Equal(paramName, exception.ParamName);
    }

    /// <summary>
    /// <see cref="ValidationHelper.ThrowIfAnyNull"/> should not throw if none
    /// of the values are null.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNull_NotThrowWhenNoValuesAreNull()
    {
        ValidationHelper.ThrowIfAnyNull(["a", "b", "c"], "param");
    }

    /// <summary>
    /// <see cref="ValidationHelper.ThrowIfAnyNullOrWhitespace"/> should throw
    /// if the collection is null.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNullOrWhitespace_ThrowsIfCollectionIsNull()
    {
        var paramName = "param";
        var exception = Assert.Throws<ArgumentNullException>(
            () => ValidationHelper.ThrowIfAnyNullOrWhitespace(null!, paramName)
        );
        Assert.Equal(
            $"Value cannot be null. (Parameter '{paramName}')",
            exception.Message
        );
        Assert.Equal(paramName, exception.ParamName);
    }

    /// <summary>
    /// <see cref="ValidationHelper.ThrowIfAnyNullOrWhitespace"/> should throw if any
    /// values are null, empty, or just whitespace.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNullOrWhitespace_ThrowsIfAnyValuesAreNullEmptyOrWhitespace()
    {
        foreach (var invalidValue in TestConstants.EmptyStringsWithNull)
        {
            var values = new string[] { "a", invalidValue!, "c" };
            var paramName = "values";

            var exception = Assert.Throws<ArgumentException>(
                () => ValidationHelper.ThrowIfAnyNullOrWhitespace(values, paramName)
            );
            Assert.Equal(
                "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'values')",
                exception.Message
            );
            Assert.Equal(paramName, exception.ParamName);
        }
    }

    /// <summary>
    /// <see cref="ValidationHelper.ThrowIfAnyNullOrWhitespace"/> should not
    /// throw if all the values are valid.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNullOrWhitespace_NotThrowWhenNoValuesAreNull()
    {
        ValidationHelper.ThrowIfAnyNullOrWhitespace(["a", "b", "c"], "param");
    }

    /// <summary>
    /// <see cref="ValidationHelper.ThrowIfNotNullAndEmptyOrWhitespace"/> should
    /// not throw if the value is null.
    /// </summary>
    [Fact]
    public void ThrowIfNotNullAndEmptyOrWhitespace_DoesNotThrowForNull()
    {
        ValidationHelper.ThrowIfNotNullAndEmptyOrWhitespace(null, "param");
    }

    /// <summary>
    /// <see cref="ValidationHelper.ThrowIfNotNullAndEmptyOrWhitespace"/> should
    /// throw the value is empty or whitespace.
    /// </summary>
    [Fact]
    public void ThrowIfNotNullAndEmptyOrWhitespace_ThrowsForEmptyOrWhitespace()
    {
        foreach (var invalidValue in TestConstants.EmptyStrings)
        {
            var paramName = "param";
            var exception = Assert.Throws<ArgumentException>(
                () => ValidationHelper.ThrowIfNotNullAndEmptyOrWhitespace(invalidValue, paramName)
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'param')",
                exception.Message
            );
            Assert.Equal(paramName, exception.ParamName);
        }
    }

    /// <summary>
    /// <see cref="ValidationHelper.ThrowIfAnyNullOrWhitespace"/> should not
    /// throw if all the values are valid.
    /// </summary>
    [Fact]
    public void ThrowIfNotNullAndEmptyOrWhitespace_DoesNotThrowForValidValue()
    {
        ValidationHelper.ThrowIfNotNullAndEmptyOrWhitespace("valid", "param");
    }
}