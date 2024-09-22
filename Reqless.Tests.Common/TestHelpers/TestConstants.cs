namespace Reqless.Tests.Common.TestHelpers;

/// <summary>
/// A variety of common contants useful for testing.
/// </summary>
public static class TestConstants
{
    /// <summary>
    /// A sample of empty strings.
    /// </summary>
    public static readonly string[] EmptyStrings = ["", " ", "\t", "\n", "\r", "  "];

    /// <summary>
    /// A sample of empty strings also including null.
    /// </summary>
    public static readonly string?[] EmptyStringsWithNull = [null, .. EmptyStrings];
}