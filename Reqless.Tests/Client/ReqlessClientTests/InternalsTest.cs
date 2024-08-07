using Reqless.Client;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for various internals of <see cref="ReqlessClient"/>.
/// </summary>
public class InternalsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="RevealingReqlessClient.GetNow"/> should return the current
    /// time in milliseconds.
    /// </summary>
    [Fact]
    public void GetNow_ReturnsTheCurrentTimeInMilliseconds()
    {
        using var subject = new RevealingReqlessClient();
        long before = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long now = subject.GetNow();
        long after = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Assert.InRange(now, before, after);
    }

    /// <summary>
    /// A test-only version of Client that reveals some of its internals for
    /// testing purposes.
    /// </summary>
    protected class RevealingReqlessClient : ReqlessClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevealingReqlessClient"/>
        /// class using the given executor instance.
        /// </summary>
        public RevealingReqlessClient() : base(true)
        {
        }

        /// <summary>
        /// Exposes access to the protected Now method for testing.
        /// </summary>
        public long GetNow() => Now();
    }
}