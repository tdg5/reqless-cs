using Reqless.Client;

namespace Reqless.Framework.Interactors;

/// <summary>
/// Concrete implementation of the <see cref="IJobInteractor"/> interface for interacting
/// with a real Reqless server.
/// </summary>
public class JobInteractor : BaseJobInteractor, IJobInteractor
{

    /// <summary>
    /// Initializes a new instance of the <see cref="JobInteractor"/> class.
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
    public JobInteractor(
        string className,
        IReqlessClient client,
        string jid,
        int priority,
        string queueName,
        int retries,
        string state,
        string[] tags,
        string[] throttles
    ) : base(
        className,
        client,
        jid,
        priority,
        queueName,
        retries,
        state,
        tags,
        throttles
    )
    {
    }
}
