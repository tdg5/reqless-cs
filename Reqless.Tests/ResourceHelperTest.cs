namespace Reqless.Tests;

/// <summary>
/// Tests for the ResourceHelper class.
/// </summary>
public class ResourceHelperTest
{
    /// <summary>
    /// The emebdded Lua script should be readable and contain expected text.
    /// </summary>
    [Fact]
    public void ReadTextResource_CanFetchTheQlessLuaScript()
    {
        var luaScript = ResourceHelper.ReadTextResource("lua/qless.lua");
        Assert.Contains("QlessAPI", luaScript);
    }

    /// <summary>
    /// An exception should be thrown if the requested resource does not exist.
    /// </summary>
    [Fact]
    public void ReadTextResource_ThrowsFileNotFoundForNonExistentResource()
    {
        var exception = Assert.Throws<FileNotFoundException>(
            () => ResourceHelper.ReadTextResource("lua/nonexistent.lua")
        );
        Assert.Equal(
            "Resource Reqless.lua.nonexistent.lua not found.",
            exception.Message
        );
    }
}