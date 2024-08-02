using Reqless.Client;
using Reqless.Models;
using Reqless.Models.JobEvents;
using Reqless.Tests.TestHelpers;
using StackExchange.Redis;

namespace Reqless.Tests.Client;

/// <summary>
/// Integration tests for <see cref="ReqlessClient"/>.
/// </summary>
public class ReqlessClientIntegrationTest
{
    private static readonly ConnectionMultiplexer _connection =
        ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");

    private readonly ReqlessClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReqlessClientIntegrationTest"/>
    /// class. Handles flushing the redis database before each test to ensure
    /// clean state.
    /// </summary>
    public ReqlessClientIntegrationTest()
    {
        _connection.GetServers().First().FlushDatabase();
        _client = new ReqlessClient(new RedisExecutor(_connection));
    }

    private static readonly string ExampleClassName = "example-class-name";

    private static readonly string ExampleData = "{}";

    private static readonly string ExampleGroupName = "example-group-name";

    private static readonly string ExampleMessage = "example-message";

    private static readonly string ExampleQueueName = "example-queue-name";

    private static readonly string ExampleThrottleName = "example-throttle-name";

    private static readonly string ExampleUpdatedData = """{"updated":true}""";

    private static readonly string ExampleWorkerName = "example-worker-name";

    /// <summary>
    /// The constructor form that takes a redis connection string should create
    /// a client successfully.
    /// </summary>
    [Fact]
    public async Task Constructor_ConnectionString_CreatesClient()
    {
        using var subject = new ReqlessClient("localhost:6379");
        Assert.NotNull(subject);
        var noSuchJob = await subject.GetJobAsync("no-such-jid");
        Assert.Null(noSuchJob);
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddDependencyToJobAsync"/> should return true
    /// when successful.
    /// </summary>
    [Fact]
    public async Task AddDependencyToJobAsync_ReturnsTrueWhenSuccessful()
    {
        var dependsOnJid = await PutJobAsync(_client);
        var jid = await PutJobAsync(
            _client,
            dependencies: Maybe<string[]>.Some([dependsOnJid])
        );
        var newDependsOnJid = await PutJobAsync(_client);
        var addedSuccessfully = await _client.AddDependencyToJobAsync(jid, newDependsOnJid);
        Assert.True(addedSuccessfully);
        var job = await _client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal([dependsOnJid, newDependsOnJid], job.Dependencies);
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddEventToJobHistoryAsync"/> should add an
    /// event to the job history.
    /// </summary>
    [Fact]
    public async Task AddEventToJobHistoryAsync_ReturnsTrueWhenSuccessful()
    {
        var jid = await PutJobAsync(_client);
        var loggedSuccessfully = await _client.AddEventToJobHistoryAsync(
            jid,
            ExampleMessage,
            data: "{\"someData\": true}"
        );
        Assert.True(loggedSuccessfully);
        var job = await _client.GetJobAsync(jid);
        Assert.NotNull(job);
        foreach (var historyEvent in job.History)
        {
            if (historyEvent is LogEvent logEvent)
            {
                Assert.True(logEvent.Data["someData"].GetBoolean());
            }
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToJobAsync"/> should return the updated
    /// list of tags.
    /// </summary>
    [Fact]
    public async Task AddTagToJobAsync_ReturnsTheUpdatedTagsList()
    {
        var initialTag = "initial-tag";
        var newTag = "new-tag";
        var jid = await PutJobAsync(
            _client,
            tags: Maybe<string[]>.Some([initialTag])
        );
        var updatedTags = await _client.AddTagToJobAsync(jid, newTag);
        var expectedTags = new string[] { initialTag, newTag };
        Assert.Equivalent(expectedTags, updatedTags);
        var job = await _client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equivalent(expectedTags, job.Tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToJobAsync"/> should return the updated
    /// list of tags.
    /// </summary>
    [Fact]
    public async Task AddTagsToJobAsync_ReturnsTheUpdatedTagsList()
    {
        var initialTag = "initial-tag";
        var newTag = "new-tag";
        var otherTag = "other-tag";
        var jid = await PutJobAsync(
            _client,
            tags: Maybe<string[]>.Some([initialTag])
        );
        var updatedTags = await _client.AddTagsToJobAsync(jid, newTag, otherTag);
        var expectedTags = new string[] { initialTag, newTag, otherTag };
        Assert.Equivalent(expectedTags, updatedTags);
        var job = await _client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equivalent(expectedTags, job.Tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToRecurringJobAsync"/> should return the
    /// updated list of tags.
    /// </summary>
    [Fact]
    public async Task AddTagToRecurringJobAsync_ReturnsTheUpdatedTagsList()
    {
        var initialTag = "initial-tag";
        var newTag = "new-tag";
        var jid = await RecurJobAsync(
            _client,
            tags: Maybe<string[]>.Some([initialTag])
        );
        var updatedTags = await _client.AddTagToRecurringJobAsync(jid, newTag);
        var expectedTags = new string[] { initialTag, newTag };
        Assert.Equivalent(expectedTags, updatedTags);
        var job = await _client.GetRecurringJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equivalent(expectedTags, job.Tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> should return the
    /// updated list of tags.
    /// </summary>
    [Fact]
    public async Task AddTagsToRecurringJobAsync_ReturnsTheUpdatedTagsList()
    {
        var initialTag = "initial-tag";
        var newTag = "new-tag";
        var otherTag = "other-tag";
        var jid = await RecurJobAsync(
            _client,
            tags: Maybe<string[]>.Some([initialTag])
        );
        var updatedTags = await _client.AddTagsToRecurringJobAsync(
            jid,
            newTag,
            otherTag
        );
        var expectedTags = new string[] { initialTag, newTag, otherTag };
        Assert.Equivalent(expectedTags, updatedTags);
        var job = await _client.GetRecurringJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equivalent(expectedTags, job.Tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobAsync"/> should return true when
    /// successful.
    /// </summary>
    [Fact]
    public async Task CancelJobAsync_ReturnsTrueWhenSuccessful()
    {
        var jid = await PutJobAsync(_client);
        var cancelledSuccessfully = await _client.CancelJobAsync(jid);
        Assert.True(cancelledSuccessfully);
        Assert.Null(await _client.GetJobAsync(jid));
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobAsync"/> should return true when there
    /// is no such job.
    /// </summary>
    [Fact]
    public async Task CancelJobAsync_ReturnsTrueIfTheJobDoesNotExist()
    {
        var cancelledSuccessfully = await _client.CancelJobAsync("no-such-jid");
        Assert.True(cancelledSuccessfully);
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobsAsync"/> should return true when
    /// successful.
    /// </summary>
    [Fact]
    public async Task CancelJobsAsync_ReturnsTrueWhenSuccessful()
    {
        var jid = await PutJobAsync(_client);
        var otherJid = await PutJobAsync(_client);
        var cancelledSuccessfully = await _client.CancelJobsAsync(jid, otherJid);
        Assert.True(cancelledSuccessfully);
        Assert.Null(await _client.GetJobAsync(jid));
        Assert.Null(await _client.GetJobAsync(otherJid));
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobsAsync"/> should return true when
    /// there is no such job.
    /// </summary>
    [Fact]
    public async Task CancelJobsAsync_ReturnsTrueIfTheJobsDoNotExist()
    {
        var cancelledSuccessfully = await _client.CancelJobsAsync(
            "no-such-jid",
            "none-such-jid"
        );
        Assert.True(cancelledSuccessfully);
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobsAsync"/> should return true if no
    /// jids are given.
    /// </summary>
    [Fact]
    public async Task CancelJobsAsync_ReturnsTrueIfNoJidsAreGiven()
    {
        var cancelledSuccessfully = await _client.CancelJobsAsync();
        Assert.True(cancelledSuccessfully);
        cancelledSuccessfully = await _client.CancelJobsAsync([]);
        Assert.True(cancelledSuccessfully);
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelRecurringJobAsync"/> should return true when
    /// successful.
    /// </summary>
    [Fact]
    public async Task CancelRecurringJobAsync_SucceedsIfTheJobExists()
    {
        var jid = await RecurJobAsync(_client);
        await _client.CancelRecurringJobAsync(jid);
        Assert.Null(await _client.GetRecurringJobAsync(jid));
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelRecurringJobAsync"/> should return true when there
    /// is no such job.
    /// </summary>
    [Fact]
    public async Task CancelRecurringJobAsync_SucceedsIfTheJobDoesNotExist()
    {
        var jid = "no-such-jid";
        Assert.Null(await _client.GetRecurringJobAsync(jid));
        await _client.CancelRecurringJobAsync(jid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteJobAsync"/> returns true upon
    /// successful completion.
    /// </summary>
    [Fact]
    public async Task CompleteJobAsync_ReturnsTrueUponCompletion()
    {
        var jid = await PutJobAsync(
            _client,
            data: Maybe<string>.Some(ExampleData),
            queueName: Maybe<string>.Some(ExampleQueueName),
            workerName: Maybe<string>.Some(ExampleWorkerName)
        );
        Job? job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        bool completedSuccessfully = await _client.CompleteJobAsync(
            data: ExampleData,
            jid: jid,
            queueName: ExampleQueueName,
            workerName: ExampleWorkerName
        );
        Assert.True(completedSuccessfully);
        var jobs = await _client.GetCompletedJobsAsync();
        Assert.Equivalent(new string[] { jid }, jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should be able to
    /// complete and requeue a job.
    /// </summary>
    [Fact]
    public async Task CompleteAndRequeueJobAsync_CanCompleteAndRequeue()
    {
        var firstQueueName = "first-queue";
        var jid = await PutJobAsync(
            _client,
            data: Maybe<string>.Some(ExampleData),
            queueName: Maybe<string>.Some(firstQueueName),
            workerName: Maybe<string>.Some(ExampleWorkerName)
        );
        Job? job = await _client.PopJobAsync(firstQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);

        var dependencyJid = await _client.PutJobAsync(
            className: ExampleClassName,
            data: ExampleData,
            queueName: ExampleQueueName,
            workerName: ExampleWorkerName
        );

        // Requeue with dependencies.
        var secondQueueName = "second-queue";
        var dependencies = new string[] { dependencyJid };
        bool completedSuccessfully = await _client.CompleteAndRequeueJobAsync(
            data: ExampleData,
            dependencies: dependencies,
            jid: jid,
            nextQueueName: secondQueueName,
            queueName: firstQueueName,
            workerName: ExampleWorkerName
        );
        Assert.True(completedSuccessfully);

        job = await _client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal("depends", job.State);
        Assert.Equal(secondQueueName, job.QueueName);
        Assert.Equal(dependencies, job.Dependencies);

        // Complete the dependency so we can requeue a third time.
        var dependencyJob = await _client.PopJobAsync(
            queueName: ExampleQueueName,
            workerName: ExampleWorkerName
        );
        Assert.NotNull(dependencyJob);
        Assert.Equal(dependencyJid, dependencyJob.Jid);

        // Requeue dependency without dependencies or delay.
        completedSuccessfully = await _client.CompleteAndRequeueJobAsync(
            data: ExampleData,
            jid: dependencyJid,
            nextQueueName: firstQueueName,
            queueName: ExampleQueueName,
            workerName: ExampleWorkerName
        );
        Assert.True(completedSuccessfully);
        dependencyJob = await _client.PopJobAsync(firstQueueName, ExampleWorkerName);
        Assert.NotNull(dependencyJob);
        Assert.Equal(dependencyJid, dependencyJob.Jid);
        completedSuccessfully = await _client.CompleteJobAsync(
            data: ExampleData,
            jid: dependencyJid,
            queueName: firstQueueName,
            workerName: ExampleWorkerName
        );
        Assert.True(completedSuccessfully);

        job = await _client.PopJobAsync(secondQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);

        // Requeue with delay.
        var thirdQueueName = "third-queue";
        completedSuccessfully = await _client.CompleteAndRequeueJobAsync(
            data: ExampleData,
            delay: 300,
            jid: jid,
            nextQueueName: thirdQueueName,
            queueName: secondQueueName,
            workerName: ExampleWorkerName
        );
        Assert.True(completedSuccessfully);

        job = await _client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal(thirdQueueName, job.QueueName);
        Assert.Equal("waiting", job.State);
        // Though the job is waiting, it should not be in the queue.
        job = await _client.PopJobAsync(thirdQueueName, ExampleWorkerName);
        Assert.Null(job);
    }

    /// <summary>
    /// <see cref="ReqlessClient.DeleteThrottleAsync"/> should delete the
    /// throttle.
    /// </summary>
    [Fact]
    public async Task DeleteThrottleAsync_DeletesTheThrottle()
    {
        var maximum = 25;
        await _client.SetThrottleAsync(ExampleThrottleName, 25);
        Throttle subject = await _client.GetThrottleAsync(ExampleThrottleName);
        Assert.Equal(maximum, subject.Maximum);
        Assert.Equal(-1, subject.Ttl);
        await _client.DeleteThrottleAsync(ExampleThrottleName);
        subject = await _client.GetThrottleAsync(ExampleThrottleName);
        Assert.Equal(0, subject.Maximum);
        Assert.Equal(-2, subject.Ttl);
    }

    /// <summary>
    /// <see cref="ReqlessClient.DeleteThrottleAsync"/> should succeed if the
    /// throttle doesn't exist.
    /// </summary>
    [Fact]
    public async Task DeleteThrottleAsync_SucceedsWhenThrottleDoesNotExist()
    {
        await _client.DeleteThrottleAsync(ExampleThrottleName);
        var subject = await _client.GetThrottleAsync(ExampleThrottleName);
        Assert.Equal(0, subject.Maximum);
        Assert.Equal(-2, subject.Ttl);
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailJobAsync"/> should return true when not
    /// given data and successful.
    /// </summary>
    [Fact]
    public async Task FailJobAsync_ReturnsTrueWhenNotGivenDataAndSuccessful()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName),
            workerName: Maybe<string>.Some(ExampleWorkerName)
        );
        Job? job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        var failedSuccessfully = await _client.FailJobAsync(
            jid,
            ExampleWorkerName,
            ExampleGroupName,
            ExampleMessage
        );
        Assert.True(failedSuccessfully);
        job = await _client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal("failed", job.State);
        Assert.NotNull(job.Failure);
        Assert.Equal(ExampleWorkerName, job.Failure.WorkerName);
        Assert.Equal(ExampleGroupName, job.Failure.Group);
        Assert.Equal(ExampleMessage, job.Failure.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailJobAsync"/> should return true when given
    /// data and successful.
    /// </summary>
    [Fact]
    public async Task FailJobAsync_ReturnsTrueWhenGivenDataAndSuccessful()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName),
            workerName: Maybe<string>.Some(ExampleWorkerName)
        );
        Job? job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        var failedSuccessfully = await _client.FailJobAsync(
            jid,
            ExampleWorkerName,
            ExampleGroupName,
            ExampleMessage,
            ExampleUpdatedData
        );
        Assert.True(failedSuccessfully);
        job = await _client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal("failed", job.State);
        Assert.Equal(ExampleUpdatedData, job.Data);
        Assert.NotNull(job.Failure);
        Assert.Equal(ExampleWorkerName, job.Failure.WorkerName);
        Assert.Equal(ExampleGroupName, job.Failure.Group);
        Assert.Equal(ExampleMessage, job.Failure.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailureGroupsCountsAsync"/> returns the counts
    /// of the various groups of
    /// failures.
    /// </summary>
    [Fact]
    public async Task FailureGroupsCountsAsync_ReturnsCountsOfVariousFailureGroups()
    {
        var initialFailedCounts = await _client.FailureGroupsCountsAsync();
        Assert.Empty(initialFailedCounts);
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName),
            workerName: Maybe<string>.Some(ExampleWorkerName)
        );
        Job? job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        var failedSuccessfully = await _client.FailJobAsync(
            jid,
            ExampleWorkerName,
            ExampleGroupName,
            ExampleMessage
        );
        Assert.True(failedSuccessfully);
        var failedCounts = await _client.FailureGroupsCountsAsync();
        Assert.Equal(
            new Dictionary<string, int> { { ExampleGroupName, 1 } },
            failedCounts
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.ForgetQueueAsync"/> should cause the named
    /// queue to be removed from the set of known queues.
    /// </summary>
    [Fact]
    public async Task ForgetQueueAsync_CausesTheNamedQueueToBeForgotten()
    {
        await PutJobAsync(_client, queueName: Maybe<string>.Some(ExampleQueueName));
        List<QueueCounts> allQueueCountsBefore = await _client.GetAllQueueCountsAsync();
        Assert.Single(allQueueCountsBefore);
        Assert.Equal(ExampleQueueName, allQueueCountsBefore[0].QueueName);
        await _client.ForgetQueueAsync(ExampleQueueName);
        List<QueueCounts> allQueueCountsAfter = await _client.GetAllQueueCountsAsync();
        Assert.Empty(allQueueCountsAfter);
    }

    /// <summary>
    /// <see cref="ReqlessClient.ForgetQueuesAsync"/> should cause the named
    /// queues to be removed from the set of known queues.
    /// </summary>
    [Fact]
    public async Task ForgetQueuesAsync_CausesTheNamedQueuesToBeForgotten()
    {
        var otherQueueName = "other-queue";
        string[] expectedQueueNames = [ExampleQueueName, otherQueueName];
        await PutJobAsync(_client, queueName: Maybe<string>.Some(ExampleQueueName));
        await PutJobAsync(_client, queueName: Maybe<string>.Some(otherQueueName));
        List<QueueCounts> allQueueCountsBefore = await _client.GetAllQueueCountsAsync();
        Assert.Equal(2, allQueueCountsBefore.Count);
        foreach (var queueCounts in allQueueCountsBefore)
        {
            Assert.Contains(queueCounts.QueueName, expectedQueueNames);

        }
        Assert.Equal(ExampleQueueName, allQueueCountsBefore[0].QueueName);
        await _client.ForgetQueuesAsync(ExampleQueueName, otherQueueName);
        List<QueueCounts> allQueueCountsAfter = await _client.GetAllQueueCountsAsync();
        Assert.Empty(allQueueCountsAfter);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllConfigsAsync "/>should return the
    /// expected default configs.
    /// </summary>
    [Fact]
    public async Task GetAllConfigsAsync_ReturnsExpectedDefaultConfigs()
    {
        var configs = await _client.GetAllConfigsAsync();

        Assert.Equal("reqless", configs["application"].GetString());
        Assert.Equal(10, configs["grace-period"].GetInt16());
        Assert.Equal(60, configs["heartbeat"].GetInt16());
        Assert.Equal(604800, configs["jobs-history"].GetInt32());
        Assert.Equal(50000, configs["jobs-history-count"].GetInt32());
        Assert.Equal(100, configs["max-job-history"].GetInt16());
        Assert.Equal(1, configs["max-pop-retry"].GetInt16());
        Assert.Equal(86400, configs["max-worker-age"].GetInt32());
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueCountsAsync"/> returns empty array
    /// when no queues exist.
    /// </summary>
    [Fact]
    public async Task GetAllQueueCountsAsync_ReturnsEmptyArrayWhenNoQueues()
    {
        var allQueueCounts = await _client.GetAllQueueCountsAsync();
        Assert.Empty(allQueueCounts);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueCountsAsync"/> returns expected
    /// counts when queues exist.
    /// </summary>
    [Fact]
    public async Task GetAllQueueCountsAsync_ReturnsExpectedCounts()
    {
        await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var allQueueCounts = await _client.GetAllQueueCountsAsync();
        Assert.Single(allQueueCounts);
        Assert.Equal(ExampleQueueName, allQueueCounts[0].QueueName);
        Assert.Equal(1, allQueueCounts[0].Waiting);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllWorkerCountsAsync"/> returns empty array
    /// when no workers exist.
    /// </summary>
    [Fact]
    public async Task GetAllWorkerCountsAsync_ReturnsEmptyArrayWhenNoWorkers()
    {
        var allWorkerCounts = await _client.GetAllWorkerCountsAsync();
        Assert.Empty(allWorkerCounts);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllWorkerCountsAsync"/> returns expected
    /// counts when workers exist.
    /// </summary>
    [Fact]
    public async Task GetAllWorkerCountsAsync_ReturnsExpectedCounts()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var poppedJid = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        var allWorkerCounts = await _client.GetAllWorkerCountsAsync();
        Assert.Single(allWorkerCounts);
        Assert.Equal(ExampleWorkerName, allWorkerCounts[0].WorkerName);
        Assert.Equal(1, allWorkerCounts[0].Jobs);
        Assert.Equal(0, allWorkerCounts[0].Stalled);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetCompletedJobsAsync"/> should return an empty
    /// list when there are no completed jobs.
    /// </summary>
    [Fact]
    public async Task GetCompletedJobsAsync_ReturnsEmptyListWhenNoSuchJobs()
    {
        var jobs = await _client.GetCompletedJobsAsync();
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetCompletedJobsAsync"/> should return expected
    /// jids when there are completed jobs.
    /// </summary>
    [Fact]
    public async Task GetCompletedJobsAsync_ReturnsExpectedCompletedJobJids()
    {
        var jid = await PutJobAsync(
            _client,
            data: Maybe<string>.Some(ExampleData),
            queueName: Maybe<string>.Some(ExampleQueueName),
            workerName: Maybe<string>.Some(ExampleWorkerName)
        );
        Job? job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        bool completedSuccessfully = await _client.CompleteJobAsync(
            data: ExampleData,
            jid: jid,
            queueName: ExampleQueueName,
            workerName: ExampleWorkerName
        );
        Assert.True(completedSuccessfully);
        var jobs = await _client.GetCompletedJobsAsync();
        Assert.Equivalent(new string[] { jid }, jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetFailedJobsByGroupAsync"/> should return an
    /// empty list when no matching jobs are found.
    /// </summary>
    [Fact]
    public async Task GetFailedJobsByGroupAsync_ReturnsEmptyListWhenNoSuchJobs()
    {
        var jidsResult = await _client.GetFailedJobsByGroupAsync("no-such-group");
        Assert.Empty(jidsResult.Jids);
        Assert.Equal(0, jidsResult.Total);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetFailedJobsByGroupAsync"/> should return
    /// expected jids when matching jobs exist.
    /// </summary>
    [Fact]
    public async Task GetFailedJobsByGroupAsync_ReturnsMatchingJidsWhenJobsExist()
    {
        var failedJobCount = 5;
        var failedJids = new string[5];

        for (var index = 0; index < failedJobCount; index++)
        {
            var jid = await PutJobAsync(
                _client,
                queueName: Maybe<string>.Some(ExampleQueueName),
                workerName: Maybe<string>.Some(ExampleWorkerName)
            );
            Job? job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
            Assert.NotNull(job);
            Assert.Equal(jid, job.Jid);
            var failedSuccessfully = await _client.FailJobAsync(
                jid,
                ExampleWorkerName,
                ExampleGroupName,
                ExampleMessage
            );
            Assert.True(failedSuccessfully);
            failedJids[index] = jid;
        }
        var jidsResult = await _client.GetFailedJobsByGroupAsync(ExampleGroupName);
        Assert.Equivalent(failedJids, jidsResult.Jids);
        Assert.Equal(failedJobCount, jidsResult.Total);

        jidsResult = await _client.GetFailedJobsByGroupAsync(
            ExampleGroupName,
            offset: failedJobCount
        );
        Assert.Empty(jidsResult.Jids);
        Assert.Equal(failedJobCount, jidsResult.Total);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobAsync "/>should return null when the job
    /// does not exist.
    /// </summary>
    [Fact]
    public async Task GetJobAsync_ReturnsNullWhenJobDoesNotExist()
    {
        var job = await _client.GetJobAsync("no-such-jid");
        Assert.Null(job);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobAsync"/>should return the job when the
    /// job exists.
    /// </summary>
    [Fact]
    public async Task GetJobAsync_ReturnsTheJobWhenItExists()
    {
        var jid = await PutJobAsync(_client);
        var job = await _client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> should return an empty array
    /// when the jobs do not exist.
    /// </summary>
    [Fact]
    public async Task GetJobsAsync_ReturnsEmptyArrayWhenJobsDoNotExist()
    {
        var jobs = await _client.GetJobsAsync(
            "no-such-jid",
            "none-such-jid"
        );
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> should return the jobs when the
    /// jobs exist.
    /// </summary>
    [Fact]
    public async Task GetJobsAsync_ReturnsTheJobsWhenTheyExist()
    {
        var jid = await PutJobAsync(_client);
        var otherJid = await PutJobAsync(_client);
        var jobs = await _client.GetJobsAsync(jid, otherJid);
        Assert.NotNull(jobs);
        Assert.Equal(2, jobs.Count);
        var expectedJids = new string[] { jid, otherJid };
        foreach (var job in jobs)
        {
            Assert.Contains(job.Jid, expectedJids);
            Assert.IsType<Job>(job);
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should return an empty
    /// list when there are no jobs in the given state.
    /// </summary>
    [Fact]
    public async Task GetJobsByStateAsync_ReturnsEmptyListWhenNoSuchJobs()
    {
        var jobs = await _client.GetJobsByStateAsync(
            queueName: ExampleQueueName,
            state: "running"
        );
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should return matching
    /// jids when there are jobs in the given state.
    /// </summary>
    [Fact]
    public async Task GetJobsByStateAsync_ReturnsJobsWhenThereAreMatchingJobs()
    {
        var jid = await PutJobAsync(
            _client,
            data: Maybe<string>.Some(ExampleData),
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        var jobs = await _client.GetJobsByStateAsync("running", ExampleQueueName);
        Assert.Equal(jid, jobs[0]);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByTagAsync"/> should return an
    /// empty list when no matching jobs are found.
    /// </summary>
    [Fact]
    public async Task GetJobsByTagAsync_ReturnsEmptyListWhenNoSuchJobs()
    {
        var jidsResult = await _client.GetJobsByTagAsync("no-such-tag");
        Assert.Empty(jidsResult.Jids);
        Assert.Equal(0, jidsResult.Total);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByTagAsync"/> should return
    /// expected jids when matching jobs exist.
    /// </summary>
    [Fact]
    public async Task GetJobsByTagAsync_ReturnsMatchingJidsWhenJobsExist()
    {
        var taggedJobCount = 5;
        var taggedJids = new string[5];
        var tag = "get-jobs-by-tag-tag";

        string[] expectedTags = [tag];
        for (var index = 0; index < taggedJobCount; index++)
        {
            var jid = await PutJobAsync(
                _client,
                queueName: Maybe<string>.Some(ExampleQueueName),
                workerName: Maybe<string>.Some(ExampleWorkerName)
            );
            var jobTags = await _client.AddTagToJobAsync(jid, tag);
            Assert.Equal(expectedTags, jobTags);
            taggedJids[index] = jid;
        }
        var jidsResult = await _client.GetJobsByTagAsync(tag);
        Assert.Equivalent(taggedJids, jidsResult.Jids);
        Assert.Equal(taggedJobCount, jidsResult.Total);

        jidsResult = await _client.GetJobsByTagAsync(tag, offset: taggedJobCount);
        Assert.Empty(jidsResult.Jids);
        Assert.Equal(taggedJobCount, jidsResult.Total);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueCountsAsync"/> returns mostly zeroes
    /// for a newly created queue.
    /// </summary>
    [Fact]
    public async Task GetQueueCountsAsync_ReturnsZeroCountsForNascentQueue()
    {
        var counts = await _client.GetQueueCountsAsync(ExampleQueueName);
        Assert.Equal(0, counts.Depends);
        Assert.Equal(ExampleQueueName, counts.QueueName);
        Assert.False(counts.Paused);
        Assert.Equal(0, counts.Recurring);
        Assert.Equal(0, counts.Running);
        Assert.Equal(0, counts.Scheduled);
        Assert.Equal(0, counts.Stalled);
        Assert.Equal(0, counts.Throttled);
        Assert.Equal(0, counts.Waiting);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueLengthAsync"/> should return zero when
    /// the queue is empty.
    /// </summary>
    [Fact]
    public async Task GetQueueLengthAsync_ReturnsZeroWhenQueueIsEmpty()
    {
        var length = await _client.GetQueueLengthAsync(ExampleQueueName);
        Assert.Equal(0, length);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueLengthAsync"/> should return expected
    /// length for non-empty queue.
    /// </summary>
    [Fact]
    public async Task GetQueueLengthAsync_ReturnsExpectedLengthWhenQueueIsNotEmpty()
    {
        var count = 10;
        for (var i = 0; i < count; i++)
        {
            await PutJobAsync(_client);
        }
        var length = await _client.GetQueueLengthAsync(ExampleQueueName);
        Assert.Equal(count, length);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueStatsAsync"/> returns stats of all
    /// zeroes for a nacent queue.
    /// </summary>
    [Fact]
    public async Task GetQueueStatsAsync_ReturnsStatsForTheQueue()
    {
        var queueStats = await _client.GetQueueStatsAsync(ExampleQueueName);
        Assert.Equal(0, queueStats.Failed);
        Assert.Equal(0, queueStats.Failures);
        Assert.Equal(0, queueStats.Retries);

        Assert.Equal(0, queueStats.Run.Count);
        Assert.Equal(0, queueStats.Run.Mean);
        Assert.Equal(0, queueStats.Run.StandardDeviation);
        Assert.Equal(148, queueStats.Run.Histogram.Length);
        foreach (var datapoint in queueStats.Run.Histogram)
        {
            Assert.Equal(0, datapoint);
        }

        Assert.Equal(0, queueStats.Wait.Count);
        Assert.Equal(0, queueStats.Wait.Mean);
        Assert.Equal(0, queueStats.Wait.StandardDeviation);
        Assert.Equal(148, queueStats.Wait.Histogram.Length);
        foreach (var datapoint in queueStats.Wait.Histogram)
        {
            Assert.Equal(0, datapoint);
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueThrottleAsync"/> should return throttle
    /// information.
    /// </summary>
    [Fact]
    public async Task GetQueueThrottleAsync_ReturnsThrottleData()
    {
        var maximum = 25;
        await _client.SetQueueThrottleAsync(ExampleQueueName, maximum);
        Throttle subject = await _client.GetQueueThrottleAsync(ExampleQueueName);
        Assert.Equal(maximum, subject.Maximum);
        Assert.Equal($"ql:q:{ExampleQueueName}", subject.Id);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> should return null
    /// if the recurring job doesn't exist.
    /// </summary>
    [Fact]
    public async Task GetRecurringJobAsync_ReturnsNullIfThereIsNoSuchRecurringJob()
    {
        RecurringJob? subject = await _client.GetRecurringJobAsync("no-such-jid");
        Assert.Null(subject);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> should be able to
    /// register recurrence and receive jid result.
    /// </summary>
    [Fact]
    public async Task GetRecurringJobAsync_CanRetrieveRecurringJobInfo()
    {
        var initialDelaySeconds = 300;
        var intervalSeconds = 180;
        var maximumBacklog = 10;
        var priority = 0;
        var retries = 5;
        var tags = new string[] { "tags" };
        var throttles = new string[] { "throttles" };

        var jid = await _client.RecurJobAtIntervalAsync(
            className: ExampleClassName,
            data: ExampleData,
            initialDelaySeconds: initialDelaySeconds,
            intervalSeconds: intervalSeconds,
            maximumBacklog: maximumBacklog,
            priority: priority,
            queueName: ExampleQueueName,
            retries: retries,
            tags: tags,
            throttles: throttles
        );

        RecurringJob? subject = await _client.GetRecurringJobAsync(jid);
        Assert.NotNull(subject);
        Assert.Equal(0, subject.Count);
        Assert.Equal(ExampleClassName, subject.ClassName);
        Assert.Equal(ExampleData, subject.Data);
        Assert.Equal(intervalSeconds, subject.IntervalSeconds);
        Assert.Equal(maximumBacklog, subject.MaximumBacklog);
        Assert.Equal(priority, subject.Priority);
        Assert.Equal(ExampleQueueName, subject.QueueName);
        Assert.Equal(retries, subject.Retries);
        Assert.Equivalent(tags, subject.Tags);
        Assert.Equivalent(throttles, subject.Throttles);
        Assert.Equal("recur", subject.State);

        await _client.CancelJobAsync(jid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleAsync"/> should return throttle
    /// information.
    /// </summary>
    [Fact]
    public async Task GetThrottleAsync_ReturnsThrottleData()
    {
        var maximum = 25;
        await _client.SetThrottleAsync(ExampleThrottleName, maximum);
        Throttle subject = await _client.GetThrottleAsync(ExampleThrottleName);
        Assert.Equal(maximum, subject.Maximum);
        Assert.Equal(ExampleThrottleName, subject.Id);
        Assert.Equal(-1, subject.Ttl);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleLockOwnersAsync"/> should return the
    /// jids that own locks for the throttle.
    /// </summary>
    [Fact]
    public async Task GetThrottleLockOwnersAsync_ReturnsJidsThatOwnLocksForTheThrottle()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName),
            throttles: Maybe<string[]>.Some([ExampleThrottleName])
        );
        await _client.SetThrottleAsync(ExampleThrottleName, 1);
        var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        List<string> lockOwners = await _client.GetThrottleLockOwnersAsync(
            ExampleThrottleName
        );
        Assert.Single(lockOwners);
        Assert.Equal(jid, lockOwners[0]);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleLockWaitersAsync"/> should return the
    /// jids that are waiting for locks for the throttle.
    /// </summary>
    [Fact]
    public async Task GetThrottleLockWaitersAsync_ReturnsJidsThatAreWaitinForTheThrottle()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName),
            throttles: Maybe<string[]>.Some([ExampleThrottleName])
        );
        await _client.SetThrottleAsync(ExampleThrottleName, 1);
        var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        var waitingJid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName),
            throttles: Maybe<string[]>.Some([ExampleThrottleName])
        );
        List<string> lockWaiters = await _client.GetThrottleLockWaitersAsync(
            ExampleThrottleName
        );
        Assert.Single(lockWaiters);
        Assert.Equal(waitingJid, lockWaiters[0]);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetTrackedJobsAsync"/> should return an empty
    /// result when no tags exist.
    /// </summary>
    [Fact]
    public async Task GetTopTagsAsync_ReturnsEmptyListWhenNoTags()
    {
        var topTags = await _client.GetTopTagsAsync();
        Assert.Empty(topTags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetTrackedJobsAsync"/> should return top tags
    /// when tags exist.
    /// </summary>
    [Fact]
    public async Task GetTopTagsAsync_ReturnsTopTags()
    {
        string[] exampleTags = ["tag-0", "tag-1", "tag-2", "tag-3", "tag-4"];
        for (var index = 0; index < exampleTags.Length; index++)
        {
            List<string> tags = [];
            for (var tagIndex = 0; tagIndex <= index; tagIndex++)
            {
                tags.Add(exampleTags[tagIndex]);
            }
            var jid = await PutJobAsync(
                _client,
                tags: Maybe<string[]>.Some([.. tags])
            );
        }

        var topTags = await _client.GetTopTagsAsync();
        // tag-4 is not included because it only appears on one job.
        Assert.Equal(
            new string[] { "tag-0", "tag-1", "tag-2", "tag-3" },
            topTags
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetTrackedJobsAsync"/> should return an empty
    /// result when no jobs are tracked.
    /// </summary>
    [Fact]
    public async Task GetTrackedJobsAsync_ReturnsEmptyListWhenNoJobs()
    {
        var trackedJobs = await _client.GetTrackedJobsAsync();
        Assert.Empty(trackedJobs.Jobs);
        Assert.Empty(trackedJobs.ExpiredJids);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetTrackedJobsAsync"/> should return expected
    /// jobs when jobs are tracked.
    /// </summary>
    [Fact]
    public async Task GetTrackedJobsAsync_ReturnsExpectedJobsWhenJobsAreTracked()
    {
        var count = 10;
        var trackedJids = new string[count];
        for (var index = 0; index < count; index++)
        {
            var jid = await PutJobAsync(_client,
                queueName: Maybe<string>.Some(ExampleQueueName)
            );
            await _client.TrackJobAsync(jid);
            trackedJids[index] = jid;
            if (index * 2 < count)
            {
                // Pop the job to simulate a worker processing it
                var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
                Assert.NotNull(job);
                Assert.Equal(jid, job.Jid);
                bool completedSuccessfully = await _client.CompleteJobAsync(
                    data: ExampleData,
                    jid: jid,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                );
                Assert.True(completedSuccessfully);
            }
        }
        var trackedJobs = await _client.GetTrackedJobsAsync();
        foreach (var trackedJob in trackedJobs.Jobs)
        {
            Assert.Contains(trackedJob.Jid, trackedJids);
        }
        Assert.Empty(trackedJobs.ExpiredJids);
    }

    /// <summary>
    /// <see cref="ReqlessClient.HeartbeatJobAsync"/> should return the new
    /// expiration time when not given data and when successful.
    /// </summary>
    [Fact]
    public async Task HeartbeatJobAsync_ReturnsTheNewExpirationTimeWithoutData()
    {
        var jid = await PutJobAsync(
            _client,
            data: Maybe<string>.Some(ExampleData),
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        await Task.Delay(5);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        var newExpiration = await _client.HeartbeatJobAsync(
            jid,
            workerName: ExampleWorkerName
        );
        Assert.True(newExpiration > job.Expires);

        var updatedJob = await _client.GetJobAsync(job.Jid);
        Assert.NotNull(updatedJob);
        // Data should not be updated
        Assert.Equal(ExampleData, updatedJob.Data);
        Assert.Equal(newExpiration, updatedJob.Expires);
    }

    /// <summary>
    /// <see cref="ReqlessClient.HeartbeatJobAsync"/> should return the new
    /// expiration time when given data and when successful.
    /// </summary>
    [Fact]
    public async Task HeartbeatJobAsync_ReturnsTheNewExpirationTimeWithData()
    {
        var jid = await PutJobAsync(
            _client,
            data: Maybe<string>.Some(ExampleData),
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        await Task.Delay(5);
        var newExpiration = await _client.HeartbeatJobAsync(
            data: ExampleUpdatedData,
            jid: jid,
            workerName: ExampleWorkerName
        );
        Assert.True(newExpiration >= job.Expires);
        var updatedJob = await _client.GetJobAsync(job.Jid);
        Assert.NotNull(updatedJob);
        Assert.Equal(ExampleUpdatedData, updatedJob.Data);
        Assert.Equal(newExpiration, updatedJob.Expires);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PauseQueueAsync"/> should pause the given queue.
    /// </summary>
    [Fact]
    public async Task PauseQueueAsync_PausesTheGivenQueue()
    {
        var countsBefore = await _client.GetQueueCountsAsync(ExampleQueueName);
        Assert.False(countsBefore.Paused);
        await _client.PauseQueueAsync(ExampleQueueName);
        var countsAfter = await _client.GetQueueCountsAsync(ExampleQueueName);
        Assert.True(countsAfter.Paused);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PeekJobsAsync"/> should return an empty array
    /// when the jobs do not exist.
    /// </summary>
    [Fact]
    public async Task PeekJobsAsync_ReturnsEmptyArrayWhenJobsDoNotExist()
    {
        var jobs = await _client.PeekJobsAsync("no-such-queue");
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PeekJobsAsync"/> should return the jobs when the
    /// jobs exist.
    /// </summary>
    [Fact]
    public async Task PeekJobsAsync_ReturnsTheJobsWhenTheyExist()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var otherJid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var jobs = await _client.PeekJobsAsync(ExampleQueueName);
        Assert.NotNull(jobs);
        Assert.Equal(2, jobs.Count);
        var expectedJids = new string[] { jid, otherJid };
        foreach (var job in jobs)
        {
            Assert.Contains(job.Jid, expectedJids);
            Assert.IsType<Job>(job);
        }

        jobs = await _client.PeekJobsAsync(ExampleQueueName, offset: 2);
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobAsync"/> should return null when there
    /// are no jobs in the queue.
    /// </summary>
    [Fact]
    public async Task PopJobAsync_ReturnsNullWhenNoJobsInQueue()
    {
        var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.Null(job);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobAsync"/> should return a job if there is
    /// a job in the queue.
    /// </summary>
    [Fact]
    public async Task PopJobAsync_ReturnsTheJobWhenOneExists()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.IsType<Job>(job);
        Assert.Equal(jid, job.Jid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobsAsync"/> should return an empty list
    /// when there are no jobs in the queue.
    /// </summary>
    [Fact]
    public async Task PopJobsAsync_ReturnsEmptyListWhenNoJobsInQueue()
    {
        var jobs = await _client.PopJobsAsync(ExampleQueueName, ExampleWorkerName, 4);
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobsAsync"/> should return jobs when there
    /// are jobs in the queue.
    /// </summary>
    [Fact]
    public async Task PopJobsAsync_ReturnsJobsWhenThereAreJobsInQueue()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var otherJid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var jobs = await _client.PopJobsAsync(
            ExampleQueueName,
            ExampleWorkerName,
            4
        );
        Assert.Equal(2, jobs.Count);
        var poppedJids = jobs.Select(job => job.Jid).ToList();
        Assert.Contains(jid, poppedJids);
        Assert.Contains(otherJid, poppedJids);
        Assert.IsType<Job>(jobs[0]);
        Assert.IsType<Job>(jobs[1]);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> should be able to put and
    /// receive jid result.
    /// </summary>
    [Fact]
    public async Task PutJobAsync_CanPutAndReceiveJid()
    {
        var dependencyJid = "dependencyJid";
        var dependencies = new string[] { dependencyJid };
        var priority = 0;
        var retries = 5;
        var tags = new string[] { "tags" };
        var throttles = new string[] { "throttles" };

        var dependencyPutJid = await _client.PutJobAsync(
            className: ExampleClassName,
            data: ExampleData,
            jid: dependencyJid,
            priority: priority,
            queueName: ExampleQueueName,
            retries: retries,
            tags: tags,
            throttles: throttles,
            workerName: ExampleWorkerName
        );

        Assert.Equal(dependencyJid, dependencyPutJid);

        var jid = await _client.PutJobAsync(
            className: ExampleClassName,
            data: ExampleData,
            dependencies: dependencies,
            priority: priority,
            queueName: ExampleQueueName,
            retries: retries,
            tags: tags,
            throttles: throttles,
            workerName: ExampleWorkerName
        );

        Job? dependency = await _client.GetJobAsync(dependencyJid);
        Assert.NotNull(dependency);
        Assert.Equivalent(new string[] { jid }, dependency.Dependents);
        Assert.Equal("waiting", dependency.State);

        Job? subject = await _client.GetJobAsync(jid);
        Assert.NotNull(subject);
        Assert.Equal(ExampleClassName, subject.ClassName);
        Assert.Equal(ExampleData, subject.Data);
        Assert.Equivalent(dependencies, subject.Dependencies);
        Assert.Equal(priority, subject.Priority);
        Assert.Equal(ExampleQueueName, subject.QueueName);
        Assert.Equal(retries, subject.Remaining);
        Assert.Equal(retries, subject.Retries);
        Assert.Equivalent(tags, subject.Tags);
        Assert.Equivalent(throttles, subject.Throttles);
        Assert.False(subject.Tracked);
        Assert.Null(subject.SpawnedFromJid);
        Assert.Null(subject.Failure);
        Assert.Equal("depends", subject.State);

        await _client.CancelJobAsync(jid);
        await _client.CancelJobAsync(dependencyJid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> should be able to
    /// register recurrence and receive jid result.
    /// </summary>
    [Fact]
    public async Task RecurJobAtIntervalAsync_CanScheduleRecurrenceAndReceiveJid()
    {
        var initialDelaySeconds = 300;
        var intervalSeconds = 180;
        var maximumBacklog = 10;
        var priority = 0;
        var retries = 5;
        var tags = new string[] { "tags" };
        var throttles = new string[] { "throttles" };

        var jid = await _client.RecurJobAtIntervalAsync(
            maximumBacklog: maximumBacklog,
            className: ExampleClassName,
            data: ExampleData,
            initialDelaySeconds: initialDelaySeconds,
            intervalSeconds: intervalSeconds,
            priority: priority,
            queueName: ExampleQueueName,
            retries: retries,
            tags: tags,
            throttles: throttles
        );

        RecurringJob? subject = await _client.GetRecurringJobAsync(jid);
        Assert.NotNull(subject);
        Assert.Equal(0, subject.Count);
        Assert.Equal(ExampleClassName, subject.ClassName);
        Assert.Equal(ExampleData, subject.Data);
        Assert.Equal(intervalSeconds, subject.IntervalSeconds);
        Assert.Equal(maximumBacklog, subject.MaximumBacklog);
        Assert.Equal(priority, subject.Priority);
        Assert.Equal(ExampleQueueName, subject.QueueName);
        Assert.Equal(retries, subject.Retries);
        Assert.Equivalent(tags, subject.Tags);
        Assert.Equivalent(throttles, subject.Throttles);
        Assert.Equal("recur", subject.State);

        await _client.CancelJobAsync(jid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.ReleaseJobThrottleAsync"/> should release the
    /// throttle for the given jobs.
    /// </summary>
    [Fact]
    public async Task ReleaseJobThrottleAsync_ReleasesTheThrottleForTheGivenJobs()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName),
            throttles: Maybe<string[]>.Some([ExampleThrottleName])
        );
        await _client.SetThrottleAsync(ExampleThrottleName, 1);
        var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        List<string> lockOwners = await _client.GetThrottleLockOwnersAsync(
            ExampleThrottleName
        );
        Assert.Single(lockOwners);
        Assert.Equal(jid, lockOwners[0]);
        var waitingJid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName),
            throttles: Maybe<string[]>.Some([ExampleThrottleName])
        );
        List<string> lockWaiters = await _client.GetThrottleLockWaitersAsync(
            ExampleThrottleName
        );
        Assert.Single(lockWaiters);
        Assert.Equal(waitingJid, lockWaiters[0]);
        await _client.ReleaseJobThrottleAsync(waitingJid, ExampleThrottleName);
        lockWaiters = await _client.GetThrottleLockWaitersAsync(
            ExampleThrottleName
        );
        Assert.Empty(lockWaiters);
        await _client.ReleaseJobThrottleAsync(jid, ExampleThrottleName);
        lockOwners = await _client.GetThrottleLockOwnersAsync(
            ExampleThrottleName
        );
        Assert.Empty(lockOwners);
    }

    /// <summary>
    /// <see cref="ReqlessClient.ReleaseThrottleForJobsAsync"/> should release the
    /// throttle for the given jobs.
    /// </summary>
    [Fact]
    public async Task ReleaseThrottleForJobsAsync_ReleasesTheThrottleForTheGivenJobs()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName),
            throttles: Maybe<string[]>.Some([ExampleThrottleName])
        );
        await _client.SetThrottleAsync(ExampleThrottleName, 1);
        var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        List<string> lockOwners = await _client.GetThrottleLockOwnersAsync(
            ExampleThrottleName
        );
        Assert.Single(lockOwners);
        Assert.Equal(jid, lockOwners[0]);
        var waitingJid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName),
            throttles: Maybe<string[]>.Some([ExampleThrottleName])
        );
        List<string> lockWaiters = await _client.GetThrottleLockWaitersAsync(
            ExampleThrottleName
        );
        Assert.Single(lockWaiters);
        Assert.Equal(waitingJid, lockWaiters[0]);
        await _client.ReleaseThrottleForJobsAsync(
            ExampleThrottleName,
            jid,
            waitingJid
        );
        lockWaiters = await _client.GetThrottleLockWaitersAsync(
            ExampleThrottleName
        );
        Assert.Empty(lockWaiters);
        lockOwners = await _client.GetThrottleLockOwnersAsync(
            ExampleThrottleName
        );
        Assert.Empty(lockOwners);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveDependencyFromJobAsync"/> should return
    /// true when successful.
    /// </summary>
    [Fact]
    public async Task RemoveDependencyFromJobAsync_ReturnsTrueWhenSuccessful()
    {
        var dependsOnJid = await PutJobAsync(_client);
        var otherDependsOnJid = await PutJobAsync(_client);
        var jid = await PutJobAsync(
            _client,
            dependencies: Maybe<string[]>.Some([dependsOnJid, otherDependsOnJid])
        );
        var removedSuccessfully = await _client.RemoveDependencyFromJobAsync(
            jid,
            otherDependsOnJid
        );
        Assert.True(removedSuccessfully);
        var job = await _client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal([dependsOnJid], job.Dependencies);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromJobAsync"/> should return the
    /// updated list of tags.
    /// </summary>
    [Fact]
    public async Task RemoveTagFromJobAsync_ReturnsTheUpdatedTagsList()
    {
        var initialTag = "initial-tag";
        var otherTag = "other-tag";
        var jid = await PutJobAsync(
            _client,
            tags: Maybe<string[]>.Some([initialTag, otherTag])
        );
        var updatedTags = await _client.RemoveTagFromJobAsync(jid, otherTag);
        var expectedTags = new string[] { initialTag };
        Assert.Equivalent(expectedTags, updatedTags);
        var job = await _client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equivalent(expectedTags, job.Tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromJobAsync"/> should return the
    /// updated list of tags.
    /// </summary>
    [Fact]
    public async Task RemoveTagsFromJobAsync_ReturnsTheUpdatedTagsList()
    {
        var initialTag = "initial-tag";
        var otherTag = "other-tag";
        var otherOtherTag = "other-other-tag";
        var jid = await PutJobAsync(
            _client,
            tags: Maybe<string[]>.Some([initialTag, otherTag, otherOtherTag])
        );
        var updatedTags = await _client.RemoveTagsFromJobAsync(
            jid,
            otherTag,
            otherOtherTag
        );
        var expectedTags = new string[] { initialTag };
        Assert.Equivalent(expectedTags, updatedTags);
        var job = await _client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equivalent(expectedTags, job.Tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should requeue the job with
    /// expected updates.
    /// </summary>
    [Fact]
    public async Task RequeueJobAsync_RequeuesAndUpdatesTheJob()
    {
        var jid = await PutJobAsync(_client);
        var jobBefore = await _client.GetJobAsync(jid);
        Assert.NotNull(jobBefore);

        var className = "new-class-name";
        var data = """{"new":true}""";
        var priority = jobBefore.Priority + 1;
        var queueName = "new-queue-name";
        var retries = jobBefore.Retries + 1;
        var tags = new string[] { "tag" };
        var throttles = new string[] { "throttle" };
        var workerName = "new-worker-name";

        var dependencyJid = await PutJobAsync(_client);
        var dependencies = new string[] { dependencyJid };

        Assert.NotEqual(className, jobBefore.ClassName);
        Assert.NotEqual(data, jobBefore.Data);
        Assert.NotEqual(dependencies, jobBefore.Dependencies);
        Assert.NotEqual(priority, jobBefore.Priority);
        Assert.NotEqual(queueName, jobBefore.QueueName);
        Assert.NotEqual(retries, jobBefore.Retries);
        Assert.NotEqual(tags, jobBefore.Tags);
        Assert.NotEqual(throttles, jobBefore.Throttles);
        Assert.NotEqual(workerName, jobBefore.WorkerName);

        var jidFromRequeue = await _client.RequeueJobAsync(
            className: className,
            data: data,
            dependencies: dependencies,
            jid: jid,
            priority: priority,
            queueName: queueName,
            retries: retries,
            tags: tags,
            throttles: throttles,
            workerName: workerName
        );
        Assert.Equal(jid, jidFromRequeue);

        Job? jobAfter = await _client.GetJobAsync(jid);
        Assert.NotNull(jobAfter);

        Assert.Equal(className, jobAfter.ClassName);
        Assert.Equal(data, jobAfter.Data);
        Assert.Equal(dependencies, jobAfter.Dependencies);
        Assert.Equal(priority, jobAfter.Priority);
        Assert.Equal(queueName, jobAfter.QueueName);
        Assert.Equal(retries, jobAfter.Retries);
        Assert.Equal(tags, jobAfter.Tags);
        // The job may have additional throttles, so we just check that the
        // expected throttles are present.
        foreach (var throttle in throttles)
        {
            Assert.Contains(throttle, jobAfter.Throttles);
        }
        Assert.Null(jobAfter.WorkerName);

        await _client.CancelJobAsync(jid);
        await _client.CancelJobAsync(dependencyJid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should return true if the job
    /// has retries remaining and job metadata should be updated to reflect the
    /// retry.
    /// </summary>
    [Fact]
    public async Task RetryJobAsync_SchedulesAJobForRetryIfTheJobHasRetriesRemaining()
    {
        var retries = 5;
        var jid = await PutJobAsync(
            _client,
            retries: Maybe<int>.Some(retries),
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.IsType<Job>(job);
        Assert.Equal(jid, job.Jid);
        Assert.Equal(retries, job.Retries);
        Assert.Equal(retries, job.Remaining);
        var retrySuccessful = await _client.RetryJobAsync(
            jid,
            ExampleQueueName,
            ExampleWorkerName,
            ExampleGroupName,
            ExampleMessage
        );
        Assert.True(retrySuccessful);
        Job? retriedJob = await _client.GetJobAsync(jid);
        Assert.NotNull(retriedJob);
        Assert.Equal("waiting", retriedJob.State);
        Assert.NotNull(retriedJob.Failure);
        Assert.Equal(ExampleWorkerName, retriedJob.Failure.WorkerName);
        Assert.Equal(ExampleGroupName, retriedJob.Failure.Group);
        Assert.Equal(ExampleMessage, retriedJob.Failure.Message);
        Assert.Equal(retries - 1, retriedJob.Remaining);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should return false if the job
    /// has no retries remaining and job metadata should be updated as expected
    /// to reflect the failure.
    /// </summary>
    [Fact]
    public async Task RetryJobAsync_DoesNotScheduleAJobForRetryIfTheJobHasNoRetriesRemaining()
    {
        var retries = 0;
        var jid = await PutJobAsync(
            _client,
            retries: Maybe<int>.Some(retries),
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.IsType<Job>(job);
        Assert.Equal(jid, job.Jid);
        Assert.Equal(retries, job.Retries);
        Assert.Equal(retries, job.Remaining);
        var retrySuccessful = await _client.RetryJobAsync(
            jid,
            ExampleQueueName,
            ExampleWorkerName,
            ExampleGroupName,
            ExampleMessage
        );
        Assert.False(retrySuccessful);
        Job? failedJob = await _client.GetJobAsync(jid);
        Assert.NotNull(failedJob);
        Assert.Equal("failed", failedJob.State);
        Assert.NotNull(failedJob.Failure);
        Assert.Equal(ExampleWorkerName, failedJob.Failure.WorkerName);
        Assert.Equal(ExampleGroupName, failedJob.Failure.Group);
        Assert.Equal(ExampleMessage, failedJob.Failure.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should retry with a delay if a
    /// delay is given.
    /// </summary>
    [Fact]
    public async Task RetryJobAsync_SchedulesARetryWithADelayWhenGivenADelay()
    {
        var retries = 5;
        var jid = await PutJobAsync(
            _client,
            retries: Maybe<int>.Some(retries),
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.IsType<Job>(job);
        Assert.Equal(jid, job.Jid);
        Assert.Equal(retries, job.Retries);
        Assert.Equal(retries, job.Remaining);
        var retrySuccessful = await _client.RetryJobAsync(
            delay: 300,
            groupName: ExampleGroupName,
            jid: jid,
            message: ExampleMessage,
            queueName: ExampleQueueName,
            workerName: ExampleWorkerName
        );
        Assert.True(retrySuccessful);
        Job? failedJob = await _client.GetJobAsync(jid);
        Assert.NotNull(failedJob);
        Assert.Equal("scheduled", failedJob.State);
        Assert.NotNull(failedJob.Failure);
        Assert.Equal(ExampleWorkerName, failedJob.Failure.WorkerName);
        Assert.Equal(ExampleGroupName, failedJob.Failure.Group);
        Assert.Equal(ExampleMessage, failedJob.Failure.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetJobPriorityAsync"/> should update the job
    /// priority.
    /// </summary>
    [Fact]
    public async Task SetJobPriorityAsync_UpdatesTheJobPriority()
    {
        var priority = 0;
        var jid = await PutJobAsync(
            _client,
            priority: Maybe<int>.Some(priority)
        );
        var newPriority = 1;
        var updatedSuccessfully = await _client.SetJobPriorityAsync(
            jid,
            newPriority
        );
        Assert.True(updatedSuccessfully);
        Job? updatedJob = await _client.GetJobAsync(jid);
        Assert.NotNull(updatedJob);
        Assert.Equal(newPriority, updatedJob.Priority);
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetQueueThrottleAsync"/> should set the
    /// throttle maximum.
    /// </summary>
    [Fact]
    public async Task SetQueueThrottleAsync_SetsThrottleMaximum()
    {
        var maximum = 25;
        await _client.SetQueueThrottleAsync(ExampleQueueName, maximum);
        Throttle subject = await _client.GetQueueThrottleAsync(ExampleQueueName);
        Assert.Equal(maximum, subject.Maximum);
        Assert.Equal($"ql:q:{ExampleQueueName}", subject.Id);
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetThrottleAsync"/> should set the
    /// throttle maximum.
    /// </summary>
    [Fact]
    public async Task SetThrottleAsync_SetsThrottleMaximumAndTtl()
    {
        var maximum = 25;
        var ttl = 300;
        await _client.SetThrottleAsync(ExampleThrottleName, maximum, ttl);
        Throttle subject = await _client.GetThrottleAsync(ExampleThrottleName);
        Assert.Equal(maximum, subject.Maximum);
        Assert.Equal(ExampleThrottleName, subject.Id);
        Assert.InRange(subject.Ttl, ttl - 30, ttl);
    }

    /// <summary>
    /// <see cref="ReqlessClient.TimeoutJobAsync"/> should cause the job to time
    /// out.
    /// </summary>
    [Fact]
    public async Task TimeoutJobAsync_TimesOutTheJob()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        await _client.TimeoutJobAsync(jid);
        Job? timedOutJob = await _client.GetJobAsync(jid);
        Assert.NotNull(timedOutJob);
        Assert.Equal("stalled", timedOutJob.State);
        Assert.Null(timedOutJob.WorkerName);
    }

    /// <summary>
    /// <see cref="ReqlessClient.TimeoutJobsAsync"/> should cause the jobs to
    /// time out.
    /// </summary>
    [Fact]
    public async Task TimeoutJobsAsync_TimesOutTheJobs()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var otherQueueName = "other-queue-name";
        var otherJid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(otherQueueName)
        );
        var job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        var otherJob = await _client.PopJobAsync(otherQueueName, ExampleWorkerName);
        Assert.NotNull(otherJob);
        Assert.Equal(otherJid, otherJob.Jid);
        await _client.TimeoutJobsAsync(jid, otherJid);

        foreach (var timedOutJid in new string[] { jid, otherJid })
        {
            Job? timedOutJob = await _client.GetJobAsync(timedOutJid);
            Assert.NotNull(timedOutJob);
            Assert.Equal("stalled", timedOutJob.State);
            Assert.Null(timedOutJob.WorkerName);
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.TrackJobAsync"/> should cause the job to become
    /// tracked.
    /// </summary>
    [Fact]
    public async Task TrackJobAsync_CausesTheJobToBeTracked()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var jobBefore = await _client.GetJobAsync(jid);
        Assert.NotNull(jobBefore);
        Assert.False(jobBefore.Tracked);

        var trackedSuccessfully = await _client.TrackJobAsync(jid);
        Assert.True(trackedSuccessfully);
        var jobAfter = await _client.GetJobAsync(jid);
        Assert.NotNull(jobAfter);
        Assert.True(jobAfter.Tracked);
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/> should unpause the given queue.
    /// </summary>
    [Fact]
    public async Task UnfailJobsFromFailureGroupIntoQueueAsync_UnpausesTheGivenQueue()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName),
            workerName: Maybe<string>.Some(ExampleWorkerName)
        );
        Job? job = await _client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        var failedSuccessfully = await _client.FailJobAsync(
            jid,
            ExampleWorkerName,
            ExampleGroupName,
            ExampleMessage
        );
        Assert.True(failedSuccessfully);
        job = await _client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal("failed", job.State);

        var otherQueueName = "other-queue-name";
        var unfailedCount = await _client.UnfailJobsFromFailureGroupIntoQueueAsync(
            count: 2,
            groupName: ExampleGroupName,
            queueName: otherQueueName
        );
        Assert.Equal(1, unfailedCount);

        job = await _client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal("waiting", job.State);
        Assert.Equal(otherQueueName, job.QueueName);
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnpauseQueueAsync"/> should unpause the given queue.
    /// </summary>
    [Fact]
    public async Task UnpauseQueueAsync_UnpausesTheGivenQueue()
    {
        await _client.PauseQueueAsync(ExampleQueueName);
        var countsBefore = await _client.GetQueueCountsAsync(ExampleQueueName);
        Assert.True(countsBefore.Paused);
        await _client.UnpauseQueueAsync(ExampleQueueName);
        var countsAfter = await _client.GetQueueCountsAsync(ExampleQueueName);
        Assert.False(countsAfter.Paused);
    }

    /// <summary>
    /// <see cref="ReqlessClient.UntrackJobAsync"/> should cause the job to become
    /// untracked.
    /// </summary>
    [Fact]
    public async Task UntrackJobAsync_CausesTheJobToBeUntracked()
    {
        var jid = await PutJobAsync(
            _client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );

        var trackedSuccessfully = await _client.TrackJobAsync(jid);
        Assert.True(trackedSuccessfully);

        var jobBefore = await _client.GetJobAsync(jid);
        Assert.NotNull(jobBefore);
        Assert.True(jobBefore.Tracked);

        var untrackedSuccessfully = await _client.UntrackJobAsync(jid);
        Assert.True(untrackedSuccessfully);
        var jobAfter = await _client.GetJobAsync(jid);
        Assert.NotNull(jobAfter);
        Assert.False(jobAfter.Tracked);
    }

    /// <summary>
    /// <see cref="ReqlessClient.UpdateRecurringJobAsync"/> should update the
    /// recurring job.
    /// </summary>
    [Fact]
    public async Task UpdateRecurringJobAsync_UpdatesTheRecurringJob()
    {
        var jid = await RecurJobAsync(_client);
        var recurringJob = await _client.GetRecurringJobAsync(jid);
        Assert.NotNull(recurringJob);
        var newClassName = "other-class-name";
        var newIntervalSeconds = recurringJob.IntervalSeconds + 60;
        var newMaximumBacklog = recurringJob.MaximumBacklog + 5;
        var newPriority = recurringJob.Priority + 10;
        var newQueueName = "other-queue-name";
        var newRetries = recurringJob.Retries + 10;
        string[] newThrottles = ["new-throttle"];
        await _client.UpdateRecurringJobAsync(
            className: newClassName,
            data: ExampleUpdatedData,
            intervalSeconds: newIntervalSeconds,
            jid: jid,
            maximumBacklog: newMaximumBacklog,
            priority: newPriority,
            queueName: newQueueName,
            retries: newRetries,
            throttles: newThrottles
        );
        recurringJob = await _client.GetRecurringJobAsync(jid);
        Assert.NotNull(recurringJob);
        Assert.Equal(newClassName, recurringJob.ClassName);
        Assert.Equal(ExampleUpdatedData, recurringJob.Data);
        Assert.Equal(newIntervalSeconds, recurringJob.IntervalSeconds);
        Assert.Equal(newMaximumBacklog, recurringJob.MaximumBacklog);
        Assert.Equal(newPriority, recurringJob.Priority);
        Assert.Equal(newQueueName, recurringJob.QueueName);
        Assert.Equal(newRetries, recurringJob.Retries);
        Assert.Equivalent(newThrottles, recurringJob.Throttles);
    }

    private static async Task<string> PutJobAsync(
        ReqlessClient _client,
        Maybe<string>? className = null,
        Maybe<string>? data = null,
        Maybe<string[]>? dependencies = null,
        Maybe<int>? priority = null,
        Maybe<string>? queueName = null,
        Maybe<int>? retries = null,
        Maybe<string[]>? tags = null,
        Maybe<string[]>? throttles = null,
        Maybe<string>? workerName = null
    )
    {
        var jid = await _client.PutJobAsync(
            className: (className ?? Maybe<string>.None).GetOrDefault(ExampleClassName),
            data: (data ?? Maybe<string>.None).GetOrDefault(ExampleData),
            dependencies: (dependencies ?? Maybe<string[]>.None).GetOrDefault([]),
            priority: (priority ?? Maybe<int>.None).GetOrDefault(0),
            retries: (retries ?? Maybe<int>.None).GetOrDefault(5),
            queueName: (queueName ?? Maybe<string>.None).GetOrDefault(ExampleQueueName),
            tags: (tags ?? Maybe<string[]>.None).GetOrDefault([]),
            throttles: (throttles ?? Maybe<string[]>.None).GetOrDefault([]),
            workerName: (workerName ?? Maybe<string>.None).GetOrDefault(ExampleWorkerName)
        );
        return jid;
    }

    private static async Task<string> RecurJobAsync(
        ReqlessClient _client,
        Maybe<string>? className = null,
        Maybe<string>? data = null,
        Maybe<int>? intervalSeconds = null,
        Maybe<int>? priority = null,
        Maybe<string>? queueName = null,
        Maybe<int>? retries = null,
        Maybe<string[]>? tags = null
    )
    {
        var jid = await _client.RecurJobAtIntervalAsync(
            className: (className ?? Maybe<string>.None).GetOrDefault(ExampleClassName),
            data: (data ?? Maybe<string>.None).GetOrDefault(ExampleData),
            intervalSeconds: (intervalSeconds ?? Maybe<int>.None).GetOrDefault(300),
            priority: (priority ?? Maybe<int>.None).GetOrDefault(0),
            retries: (retries ?? Maybe<int>.None).GetOrDefault(5),
            queueName: (queueName ?? Maybe<string>.None).GetOrDefault(ExampleQueueName),
            tags: (tags ?? Maybe<string[]>.None).GetOrDefault([])
        );
        return jid;
    }
}
