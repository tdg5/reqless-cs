using Moq;
using Reqless.Client;
using Reqless.Common.Utilities;
using Reqless.Framework;
using Reqless.Framework.Interactors;

namespace Reqless.Tests.TestHelpers.Framework.Interactors;

/// <summary>
/// Factory for creating instances of <see cref="JobInteractor"/> for testing.
/// </summary>
public static class JobInteractorFactory
{
    /// <summary>
    /// Create an instance of <see cref="JobInteractor"/> for testing.
    /// </summary>
    /// <param name="className">The name of the <see cref="IUnitOfWork"/> class
    /// that will be used to perform the job.</param>
    /// <param name="client">An <see cref="IReqlessClient"/> that the job can use to
    /// interact with the Reqless server.</param>
    /// <param name="jid">The unique identifier, or job ID, of the job.</param>
    /// <param name="priority">The priority of the job.</param>
    /// <param name="queueName">The name of the queue that the job currently
    /// belongs to.</param>
    /// <param name="retries">The total number of times the job will been
    /// retried before the job is declared failed.</param>
    /// <param name="state">The current state of the job.</param>
    /// <param name="tags">A list of tags that are applied to the job for
    /// tracking purposes.</param>
    /// <param name="throttles">A list of throttles that are applied to the job
    /// to manage various concurrency limits and prevent the job from being
    /// scheduled when capacity is not available.</param>
    public static JobInteractor NewJobInteractor(
        Maybe<string>? className = null,
        Maybe<IReqlessClient>? client = null,
        Maybe<string>? jid = null,
        Maybe<int>? priority = null,
        Maybe<string>? queueName = null,
        Maybe<int>? retries = null,
        Maybe<string>? state = null,
        Maybe<string[]>? tags = null,
        Maybe<string[]>? throttles = null
    )
    {
        var _className = className ?? Maybe.Some("default-class-name");
        var _client = client ?? Maybe.Some(new Mock<IReqlessClient>().Object);
        var _jid = jid ?? Maybe.Some("default-jid");
        var _priority = priority ?? Maybe<int>.Some(0);
        var _queueName = queueName ?? Maybe.Some("default-queue-name");
        var _retries = retries ?? Maybe<int>.Some(0);
        var _state = state ?? Maybe.Some("default-state");
        var _tags = tags ?? Maybe<string[]>.Some([]);
        var _throttles = throttles ?? Maybe<string[]>.Some([]);

        return new JobInteractor(
            className: _className.GetOrDefault(null!),
            client: _client.GetOrDefault(null!),
            jid: _jid.GetOrDefault(null!),
            priority: _priority.GetOrDefault(0),
            queueName: _queueName.GetOrDefault(null!),
            retries: _retries.GetOrDefault(0),
            state: _state.GetOrDefault(null!),
            tags: _tags.GetOrDefault(null!),
            throttles: _throttles.GetOrDefault(null!)
        );
    }
}
