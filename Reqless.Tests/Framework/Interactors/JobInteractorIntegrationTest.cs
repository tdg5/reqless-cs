using Reqless.Client;
using Reqless.Framework.Interactors;
using Reqless.Tests.TestHelpers.Client;

namespace Reqless.Tests.Framework.Interactors;

/// <summary>
/// Integration tests for the <see cref="JobInteractor"/> class.
/// </summary>
public class JobInteractorIntegrationTest
{
    private readonly IReqlessClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobInteractorIntegrationTest"/>
    /// class with a real <see cref="ReqlessClient"/> instance.
    /// </summary>
    public JobInteractorIntegrationTest()
    {
        _client = ClientFactory.Client(
            flushDatabase: true
        );
    }
}
