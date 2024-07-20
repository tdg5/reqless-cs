namespace Reqless.Tests.TestHelpers;

static class TestConstants
{
    public static readonly string[] EmptyStrings = ["", " ", "\t", "\n", "\r", "  "];

    public static readonly string?[] EmptyStringsWithNull = [null, .. EmptyStrings];
}