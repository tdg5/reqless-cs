using Reqless.Common.Validation;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Common.Validation;

/// <summary>
/// Tests for the <see cref="OperationValidation"/> class.
/// </summary>
public class OperationValidationTest
{
    /// <summary>
    /// <see cref="OperationValidation.ThrowIfNull"/> should throw if the
    /// subject is null.
    /// </summary>
    [Fact]
    public void ThrowIfNull_ThrowsIfSubjectIsNull()
    {
        string? subject = null;
        var subjectName = "subject-name";
        Scenario.ThrowsWhenOperationEncountersNull(
            () => OperationValidation.ThrowIfNull(subject, subjectName), subjectName);
    }

    /// <summary>
    /// <see cref="OperationValidation.ThrowIfNull"/> should not throw if the
    /// subject is not null.
    /// </summary>
    [Fact]
    public void ThrowIfNull_DoesNotThrowIfSubjectIsNotNull()
    {
        OperationValidation.ThrowIfNull("not null", "subject-name");
    }

    /// <summary>
    /// <see cref="OperationValidation.ThrowIfAnyNull"/> should throw if the
    /// collection is null.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNull_ThrowsIfCollectionIsNull()
    {
        var subjectName = "subject";
        Scenario.ThrowsWhenOperationEncountersNull(
            () => OperationValidation.ThrowIfAnyNull<string>(null!, subjectName),
            subjectName);
    }

    /// <summary>
    /// <see cref="OperationValidation.ThrowIfAnyNull"/> should throw if any
    /// values are null.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNull_ThrowsIfAnyValuesAreNull()
    {
        var values = new string?[] { "a", null, "c" };
        var subjectName = "values";

        Scenario.ThrowsWhenOperationEncountersNullElement(
            () => OperationValidation.ThrowIfAnyNull(values, subjectName),
            subjectName);
    }

    /// <summary>
    /// <see cref="OperationValidation.ThrowIfAnyNull"/> should not throw if
    /// none of the values are null.
    /// </summary>
    [Fact]
    public void ThrowIfAnyNull_NotThrowWhenNoValuesAreNull()
    {
        OperationValidation.ThrowIfAnyNull(["a", "b", "c"], "subject");
    }

    /// <summary>
    /// <see cref="OperationValidation.ThrowIfServerResponseIsNull{T}(T)"/> should
    /// throw if the server response is null and a reference type.
    /// </summary>
    [Fact]
    public void ThrowIfServerResponseIsNull_ThrowsIfReferernceIsNull()
    {
        Scenario.ThrowsWhenServerRespondsWithNull(
            () => OperationValidation.ThrowIfServerResponseIsNull<string>(null));
    }

    /// <summary>
    /// <see cref="OperationValidation.ThrowIfServerResponseIsNull{T}(T)"/> should
    /// not throw if the server response is not null and a reference type.
    /// </summary>
    [Fact]
    public void ThrowIfServerResponseIsNull_DoesNotThrowIfReferernceIsNotNull()
    {
        OperationValidation.ThrowIfServerResponseIsNull("not null");
    }

    /// <summary>
    /// <see cref="OperationValidation.ThrowIfServerResponseIsNull{T}(T)"/> should
    /// throw if the server response is null and a value type.
    /// </summary>
    [Fact]
    public void ThrowIfServerResponseIsNull_ThrowsIfNullableValueTypeIsNull()
    {
        Scenario.ThrowsWhenServerRespondsWithNull(
            () => OperationValidation.ThrowIfServerResponseIsNull<int>(null));
    }

    /// <summary>
    /// <see cref="OperationValidation.ThrowIfServerResponseIsNull{T}(T)"/> should
    /// not throw if the server response is not null and a value type.
    /// </summary>
    [Fact]
    public void ThrowIfServerResponseIsNull_DoesNotThrowIfNullableValueTypeIsNotNull()
    {
        OperationValidation.ThrowIfServerResponseIsNull((int?)42);
    }
}
