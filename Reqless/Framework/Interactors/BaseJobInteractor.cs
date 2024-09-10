using Reqless.Client;
using Reqless.Common.Validation;

namespace Reqless.Framework.Interactors;

/// <summary>
/// Concrete implementation of shared job functionality for interacting with a
/// real Reqless server.
/// </summary>
/// <remarks>
/// Abstract because only specialized versions of this class should be
/// instantiated.
/// </remarks>
abstract public class BaseJobInteractor
{
    /// <inheritdoc />
    public string ClassName { get; }

    /// <summary>
    /// An <see cref="IClient"/> instance that the job can use to interact with
    /// the Reqless server.
    /// </summary>
    protected IClient Client { get; }

    /// <inheritdoc />
    public string Jid { get; }

    /// <inheritdoc />
    public int Priority { get; }

    /// <inheritdoc />
    public string QueueName { get; }

    /// <inheritdoc />
    public int Retries { get; }

    /// <inheritdoc />
    public string State { get; }

    /// <inheritdoc />
    public string[] Tags { get; }

    /// <inheritdoc />
    public string[] Throttles { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseJobInteractor"/> class.
    /// </summary>
    /// <param name="className">The name of the <see cref="IUnitOfWork"/> class
    /// that will be used to perform the job.</param>
    /// <param name="client">An <see cref="IClient"/> that the job can use to
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
    public BaseJobInteractor(
        string className,
        IClient client,
        string jid,
        int priority,
        string queueName,
        int retries,
        string state,
        string[] tags,
        string[] throttles
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(className, nameof(className));
        ArgumentNullException.ThrowIfNull(client, nameof(client));
        ArgumentException.ThrowIfNullOrWhiteSpace(jid, nameof(jid));
        ArgumentValidation.ThrowIfNegative(priority, nameof(priority));
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName, nameof(queueName));
        ArgumentValidation.ThrowIfNegative(retries, nameof(retries));
        ArgumentException.ThrowIfNullOrWhiteSpace(state, nameof(state));
        ArgumentValidation.ThrowIfAnyNullOrWhitespace(tags, nameof(tags));
        ArgumentValidation.ThrowIfAnyNullOrWhitespace(throttles, nameof(throttles));

        ClassName = className;
        Client = client;
        Jid = jid;
        Priority = priority;
        QueueName = queueName;
        Retries = retries;
        State = state;
        Tags = tags;
        Throttles = throttles;
    }
}