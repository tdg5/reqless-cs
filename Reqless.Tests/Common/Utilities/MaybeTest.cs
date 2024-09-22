using Reqless.Common.Utilities;

namespace Reqless.Tests.Common.Utilities;

/// <summary>
/// Tests for the <see cref="Maybe{T}"/> class.
/// </summary>
public class MaybeTest
{
    /// <summary>
    /// <see cref="Maybe.Some"/> should return a <see cref="Maybe{T}.Some"/> of
    /// the expected type.
    /// </summary>
    [Fact]
    public void NonGenericSome_ReturnsSomeOfExpectedType()
    {
        var value = "expected_value";
        var some = Maybe.Some(value);
        Assert.IsAssignableFrom<Maybe<string>>(some);
        Assert.Equal(value, some.GetOrDefault("unreachable"));
    }

    /// <summary>
    /// Any non-null value can be wrapped in a <see cref="Maybe{T}"/> instance.
    /// </summary>
    [Fact]
    public void ExplicitCastToSome_WorksForNonNullInstances()
    {
        var stringSomeValue = "test";
        var stringSome = (Maybe<string>)stringSomeValue;
        Assert.Equal(stringSomeValue, stringSome.GetOrDefault(null!));

        var intSomeValue = 42;
        var intSome = (Maybe<int>)intSomeValue;
        Assert.Equal(intSomeValue, intSome.GetOrDefault(0));
    }

    /// <summary>
    /// Values that are null or of type Object cannot be explicitly cast to
    /// Maybe{T} because the built-in conversions take precedence.
    /// </summary>
    [Fact]
    public void ExplicitCastToSome_DoesNotWorkForNullTypesOrObjectTypeValues()
    {
        // The cast appears to succeed, but it's not really a Maybe<object?>
        // instance. It's just null.
        var notAReferenceMaybe = ((Maybe<object?>)null!);
        Assert.Throws<NullReferenceException>(
            () => notAReferenceMaybe.GetOrDefault(null!)
        );
        Assert.Null(notAReferenceMaybe);

        // The cast appears to succeed, but it's not really a Maybe<int?>
        // instance. It's just null.
        var notAValueMaybe = ((Maybe<int?>)null!);
        Assert.Throws<NullReferenceException>(
            () => notAValueMaybe.GetOrDefault(null!)
        );
        Assert.Null(notAValueMaybe);

        // The generic implementation hides this but, user-defined conversions
        // to values of a base class are not allowed since such an operator is
        // never needed. See
        // https://learn.microsoft.com/en-us/dotnet/csharp/misc/cs0553.
        Assert.Throws<InvalidCastException>(() => (Maybe<object>)new object());
    }

    /// <summary>
    /// Any value can be made into a Maybe{T}.Some instance.
    /// </summary>
    [Fact]
    public void Static_Some_WorksForAnyValue()
    {
        var objectSomeValue = new object();
        var objectSome = Maybe<object>.Some(objectSomeValue);
        Assert.Equal(objectSomeValue, objectSome.GetOrDefault(null!));

        object? nullReferenceSomeValue = null;
        var nullReferenceSome = Maybe<object?>.Some(nullReferenceSomeValue);
        Assert.Null(nullReferenceSome.GetOrDefault(new object()));

        var stringSomeValue = "test";
        var stringSome = Maybe<string>.Some(stringSomeValue);
        Assert.Equal(stringSomeValue, stringSome.GetOrDefault("unreachable"));

        var intSomeValue = 42;
        var intSome = Maybe<int>.Some(intSomeValue);
        Assert.Equal(intSomeValue, intSome.GetOrDefault(0));

        int? nullIntSomeValue = null;
        var nullIntSome = Maybe<int?>.Some(nullIntSomeValue);
        Assert.Null(nullIntSome.GetOrDefault(0));
    }

    /// <summary>
    /// Maybe{T}.None should return a None option instance.
    /// </summary>
    [Fact]
    public void Static_None_ReturnsNoneOption()
    {
        var none = Maybe<object>.None;
        Assert.False(none.HasValue);
        Assert.True(none.IsEmpty);
    }

    /// <summary>
    /// Maybe{T}.Some.GetOrDefault should return the value.
    /// </summary>
    [Fact]
    public void Some_GetOrDefault_ReturnsValue()
    {
        var value = "expected_value";
        var subject = Maybe<string>.Some(value);
        Assert.Equal(value, subject.GetOrDefault("unreachable"));
    }

    /// <summary>
    /// Maybe{T}.Some.HasValue should return true;
    /// </summary>
    [Fact]
    public void Some_HasValue_ReturnsTrue()
    {
        var subject = Maybe<string>.Some("some");
        Assert.True(subject.HasValue);
    }

    /// <summary>
    /// Maybe{T}.Some.IsEmpty should return false;
    /// </summary>
    [Fact]
    public void Some_IsEmpty_ReturnsTrue()
    {
        var subject = Maybe<string>.Some("some");
        Assert.False(subject.IsEmpty);
    }

    /// <summary>
    /// Maybe{T}.Some.Match executes the some function with the value and
    /// returns the expected result.
    /// </summary>
    [Fact]
    public void Some_Match_ExecutesSomeFunctionWithValue()
    {
        var noneFuncCalled = false;
        var someFuncCalled = false;
        var expectedValue = "some";
        string actualValue = "not_expected";
        var expectedResult = "expected_result";
        var subject = Maybe<string>.Some(expectedValue);
        var result = subject.Match(
            someFunc: value =>
            {
                someFuncCalled = true;
                actualValue = value;
                return expectedResult;
            },
            noneFunc: () =>
            {
                noneFuncCalled = true;
                return "not_expected";
            }
        );
        Assert.True(someFuncCalled);
        Assert.False(noneFuncCalled);
        Assert.Equal(expectedValue, actualValue);
        Assert.Equal(expectedResult, result);
    }

    /// <summary>
    /// Maybe{T}.Some.Map executes the some function with the value and returns
    /// the expected result.
    /// </summary>
    [Fact]
    public void Some_Map_ExecutesSomeFunctionWithValue()
    {
        var expectedValue = "some";
        string actualValue = "not_expected";
        var expectedResult = 42;
        var subject = Maybe<string>.Some(expectedValue);
        var result = subject.Map<int>(value =>
        {
            actualValue = value;
            return expectedResult;
        }
        );
        Assert.Equal(expectedValue, actualValue);
        Assert.Equal(expectedResult, result.GetOrDefault(0));
    }

    /// <summary>
    /// Maybe{T}.None.GetOrDefault should return the default.
    /// </summary>
    [Fact]
    public void None_GetOrDefault_ReturnsValue()
    {
        var value = "expected_value";
        var subject = Maybe<string>.None;
        Assert.Equal(value, subject.GetOrDefault(value));
    }

    /// <summary>
    /// Maybe{T}.None.HasValue should return false;
    /// </summary>
    [Fact]
    public void None_HasValue_ReturnsFalse()
    {
        var subject = Maybe<string>.None;
        Assert.False(subject.HasValue);
    }

    /// <summary>
    /// Maybe{T}.None.IsEmpty should return true;
    /// </summary>
    [Fact]
    public void None_IsEmpty_ReturnsTrue()
    {
        var subject = Maybe<string>.None;
        Assert.True(subject.IsEmpty);
    }

    /// <summary>
    /// Maybe{T}.None.Match executes the none function.
    /// </summary>
    [Fact]
    public void None_Match_ExecutesNoneFunction()
    {
        var noneFuncCalled = false;
        var someFuncCalled = false;
        var expectedResult = "expected_result";
        var subject = Maybe<string>.None;
        var result = subject.Match(
            someFunc: _ =>
            {
                someFuncCalled = true;
                return "not_expected";
            },
            noneFunc: () =>
            {
                noneFuncCalled = true;
                return expectedResult;
            }
        );
        Assert.False(someFuncCalled);
        Assert.True(noneFuncCalled);
        Assert.Equal(expectedResult, result);
    }

    /// <summary>
    /// Maybe{T}.None.Map doesn't execute the function and returns a None of the
    /// expected type.
    /// </summary>
    [Fact]
    public void None_Map_ReturnsANoneOfTheExpectedType()
    {
        var mapFunctionCalled = false;
        var subject = Maybe<string>.None;
        var result = subject.Map<int>(value =>
        {
            mapFunctionCalled = true;
            return 42;
        }
        );
        Assert.False(mapFunctionCalled);
        Assert.Equal(Maybe<int>.None, result);
    }
}