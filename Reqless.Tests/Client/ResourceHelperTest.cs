using Reqless.Client;

namespace Reqless.Tests.Client;

/// <summary>
/// Tests for the ResourceHelper class.
/// </summary>
public class ResourceHelperTest
{
    /// <summary>
    /// The emebdded Lua script should be readable and contain expected text.
    /// </summary>
    [Fact]
    public void ReadTextResource_CanFetchTheReqlessLuaScript()
    {
        var luaScript = ResourceHelper.ReadTextResource("LuaScripts/reqless.lua");
        Assert.Contains("ReqlessAPI", luaScript);
    }

    /// <summary>
    /// An exception should be thrown if the requested resource does not exist.
    /// </summary>
    [Fact]
    public void ReadTextResource_ThrowsFileNotFoundForNonExistentResource()
    {
        var exception = Assert.Throws<FileNotFoundException>(
            () => ResourceHelper.ReadTextResource("LuaScripts/nonexistent.lua"));
        Assert.Equal(
            "Resource Reqless.Client.LuaScripts.nonexistent.lua not found.",
            exception.Message);
    }
}
