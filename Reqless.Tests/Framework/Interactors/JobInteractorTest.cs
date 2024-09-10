using Reqless.Client;
using Reqless.Framework.Interactors;
using Reqless.Tests.TestHelpers;
using Reqless.Tests.TestHelpers.Factories.Framework.Interactors;

namespace Reqless.Tests.Framework.Interactors;

/// <summary>
/// Unit tests for the <see cref="JobInteractor"/> class.
/// </summary>
public class JobInteractorTest
{
    /// <summary>
    /// The constructor should throw an exception when the class name argument
    /// is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ClassName_ThrowsWhenNullEmptyOrWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidClassName) =>
                MakeSubject(className: Maybe<string>.Some(invalidClassName!)),
            "className"
        );
    }

    /// <summary>
    /// The constructor should throw an exception when the client argument is
    /// null.
    /// </summary>
    [Fact]
    public void Constructor_Client_ThrowsWhenNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => MakeSubject(client: Maybe<IClient>.Some(null!)),
            "client"
        );
    }

    /// <summary>
    /// The constructor should throw an exception when the jid argument is null,
    /// empty, or whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Jid_ThrowsWhenNullEmptyOrWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidJid) => MakeSubject(jid: Maybe<string>.Some(invalidJid!)),
            "jid"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given priority argument
    /// is negative.
    /// </summary>
    [Fact]
    public void Constructor_Priority_ThrowsWhenNegative()
    {
        Scenario.ThrowsWhenArgumentIsNegative(
            (invalidPriority) => MakeSubject(
                priority: Maybe<int>.Some(invalidPriority)
            ),
            "priority"
        );
    }

    /// <summary>
    /// The constructor should throw an exception when the queue name argument
    /// is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public void Constructor_QueueName_ThrowsWhenNullEmptyOrWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidQueueName) =>
                MakeSubject(queueName: Maybe<string>.Some(invalidQueueName!)),
            "queueName"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given retries argument
    /// is negative.
    /// </summary>
    [Fact]
    public void Constructor_Retries_ThrowsWhenNegative()
    {
        Scenario.ThrowsWhenArgumentIsNegative(
            (invalidRetries) => MakeSubject(
                retries: Maybe<int>.Some(invalidRetries)
            ),
            "retries"
        );
    }

    /// <summary>
    /// The constructor should throw an exception when the state argument is
    /// null, empty, or whitespace.
    /// </summary>
    [Fact]
    public void Constructor_State_ThrowsWhenNullEmptyOrWhitespace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidState) => MakeSubject(state: Maybe<string>.Some(invalidState!)),
            "state"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given tags argument is
    /// null.
    /// </summary>
    [Fact]
    public void Constructor_Tags_ThrowsWhenNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => MakeSubject(tags: Maybe<string[]>.Some(null!)),
            "tags"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given tags include
    /// values that are null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Tags_ThrowsWhenAnyValueIsNullEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespace(
            (invalidTag) => MakeSubject(tags: Maybe<string[]>.Some([invalidTag!])),
            "tags"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given throttles
    /// argument is null.
    /// </summary>
    [Fact]
    public void Constructor_Throttles_ThrowsWhenNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => MakeSubject(throttles: Maybe<string[]>.Some(null!)),
            "throttles"
        );
    }

    /// <summary>
    /// The constructor should throw an exception if the given throttles include
    /// values that are null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public void Constructor_Throttles_ThrowsWhenAnyValueIsNullEmptyOrOnlyWhitespace()
    {
        Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespace(
            (invalidThrottle) => MakeSubject(
                throttles: Maybe<string[]>.Some([invalidThrottle!])
            ),
            "throttles"
        );
    }

    /// <summary>
    /// Create an instance of <see cref="JobInteractor"/> for testing.
    /// </summary>
    /// <returns></returns>
    static JobInteractor MakeSubject(
        Maybe<string>? className = null,
        Maybe<IClient>? client = null,
        Maybe<string>? jid = null,
        Maybe<int>? priority = null,
        Maybe<string>? queueName = null,
        Maybe<int>? retries = null,
        Maybe<string>? state = null,
        Maybe<string[]>? tags = null,
        Maybe<string[]>? throttles = null
    )
    {
        return JobInteractorFactory.NewJobInteractor(
            className: className,
            client: client,
            jid: jid,
            priority: priority,
            queueName: queueName,
            retries: retries,
            state: state,
            tags: tags,
            throttles: throttles
        );
    }
}