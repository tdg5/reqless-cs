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

    /// <summary>
    /// Initializes a new instance of the <see cref="ReqlessClientIntegrationTest"/>
    /// class. Handles flushing the redis database before each test to ensure
    /// clean state.
    /// </summary>
    public ReqlessClientIntegrationTest()
    {
        _connection.GetServers().First().FlushDatabase();
    }

    private static readonly string ExampleClassName = "example-class-name";

    private static readonly string ExampleData = "{}";

    private static readonly string ExampleGroup = "example-group";

    private static readonly string ExampleMessage = "example-message";

    private static readonly string ExampleQueueName = "example-queue-name";

    private static readonly string ExampleUpdatedData = """{"updated":true}""";

    private static readonly string ExampleWorkerName = "example-worker-name";

    /// <summary>
    /// The constructor form that takes a redis connection string should create
    /// a client successfully.
    /// </summary>
    [Fact]
    public async void Constructor_ConnectionString_CreatesClient()
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
    public async void AddDependencyToJobAsync_ReturnsTrueWhenSuccessful()
    {
        using var client = MakeClientFromConnection(_connection);
        var dependsOnJid = await PutJobAsync(client);
        var jid = await PutJobAsync(
            client,
            dependencies: Maybe<string[]>.Some([dependsOnJid])
        );
        var newDependsOnJid = await PutJobAsync(client);
        var addedSuccessfully = await client.AddDependencyToJobAsync(jid, newDependsOnJid);
        Assert.True(addedSuccessfully);
        var job = await client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal([dependsOnJid, newDependsOnJid], job.Dependencies);
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddEventToJobHistoryAsync"/> should add an
    /// event to the job history.
    /// </summary>
    [Fact]
    public async void AddEventToJobHistoryAsync_ReturnsTrueWhenSuccessful()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(client);
        var loggedSuccessfully = await client.AddEventToJobHistoryAsync(
            jid,
            ExampleMessage,
            data: "{\"someData\": true}"
        );
        Assert.True(loggedSuccessfully);
        var job = await client.GetJobAsync(jid);
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
    public async void AddTagToJobAsync_ReturnsTheUpdatedTagsList()
    {
        using var client = MakeClientFromConnection(_connection);
        var initialTag = "initial-tag";
        var newTag = "new-tag";
        var jid = await PutJobAsync(
            client,
            tags: Maybe<string[]>.Some([initialTag])
        );
        var updatedTags = await client.AddTagToJobAsync(jid, newTag);
        var expectedTags = new string[] { initialTag, newTag };
        Assert.Equivalent(expectedTags, updatedTags);
        var job = await client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equivalent(expectedTags, job.Tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToJobAsync"/> should return the updated
    /// list of tags.
    /// </summary>
    [Fact]
    public async void AddTagsToJobAsync_ReturnsTheUpdatedTagsList()
    {
        using var client = MakeClientFromConnection(_connection);
        var initialTag = "initial-tag";
        var newTag = "new-tag";
        var otherTag = "other-tag";
        var jid = await PutJobAsync(
            client,
            tags: Maybe<string[]>.Some([initialTag])
        );
        var updatedTags = await client.AddTagsToJobAsync(jid, newTag, otherTag);
        var expectedTags = new string[] { initialTag, newTag, otherTag };
        Assert.Equivalent(expectedTags, updatedTags);
        var job = await client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equivalent(expectedTags, job.Tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobsAsync"/> should throw if jids
    /// argument is null.
    /// </summary>
    [Fact]
    public async void CancelJobsAsync_ThrowsIfJidsArgumentIsNull()
    {
        using var client = MakeClientFromConnection(_connection);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => client.CancelJobsAsync(null!)
        );
        Assert.Equal("Value cannot be null. (Parameter 'jids')", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobAsync"/> should return true when
    /// successful.
    /// </summary>
    [Fact]
    public async void CancelJobAsync_ReturnsTrueWhenSuccessful()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(client);
        var cancelledSuccessfully = await client.CancelJobAsync(jid);
        Assert.True(cancelledSuccessfully);
        Assert.Null(await client.GetJobAsync(jid));
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobAsync"/> should return true when there
    /// is no such job.
    /// </summary>
    [Fact]
    public async void CancelJobAsync_ReturnsTrueIfTheJobDoesNotExist()
    {
        using var client = MakeClientFromConnection(_connection);
        var cancelledSuccessfully = await client.CancelJobAsync("no-such-jid");
        Assert.True(cancelledSuccessfully);
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobsAsync"/> should return true when
    /// successful.
    /// </summary>
    [Fact]
    public async void CancelJobsAsync_ReturnsTrueWhenSuccessful()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(client);
        var otherJid = await PutJobAsync(client);
        var cancelledSuccessfully = await client.CancelJobsAsync(jid, otherJid);
        Assert.True(cancelledSuccessfully);
        Assert.Null(await client.GetJobAsync(jid));
        Assert.Null(await client.GetJobAsync(otherJid));
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobsAsync"/> should return true when
    /// there is no such job.
    /// </summary>
    [Fact]
    public async void CancelJobsAsync_ReturnsTrueIfTheJobsDoNotExist()
    {
        using var client = MakeClientFromConnection(_connection);
        var cancelledSuccessfully = await client.CancelJobsAsync(
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
    public async void CancelJobsAsync_ReturnsTrueIfNoJidsAreGiven()
    {
        using var client = MakeClientFromConnection(_connection);
        var cancelledSuccessfully = await client.CancelJobsAsync();
        Assert.True(cancelledSuccessfully);
        cancelledSuccessfully = await client.CancelJobsAsync([]);
        Assert.True(cancelledSuccessfully);
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteJobAsync"/> returns true upon
    /// successful completion.
    /// </summary>
    [Fact]
    public async void CompleteJobAsync_ReturnsTrueUponCompletion()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(
            client,
            data: Maybe<string>.Some(ExampleData),
            queueName: Maybe<string>.Some(ExampleQueueName),
            workerName: Maybe<string>.Some(ExampleWorkerName)
        );
        Job? job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        bool completedSuccessfully = await client.CompleteJobAsync(
            data: ExampleData,
            jid: jid,
            queueName: ExampleQueueName,
            workerName: ExampleWorkerName
        );
        Assert.True(completedSuccessfully);
        var jobs = await client.GetCompletedJobsAsync();
        Assert.Equivalent(new string[] { jid }, jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should be able to
    /// complete and requeue a job.
    /// </summary>
    [Fact]
    public async void CompleteAndRequeueJobAsync_CanCompleteAndRequeue()
    {
        using var client = MakeClientFromConnection(_connection);
        var firstQueueName = "first-queue";
        var jid = await PutJobAsync(
            client,
            data: Maybe<string>.Some(ExampleData),
            queueName: Maybe<string>.Some(firstQueueName),
            workerName: Maybe<string>.Some(ExampleWorkerName)
        );
        Job? job = await client.PopJobAsync(firstQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);

        var dependencyJid = await client.PutJobAsync(
            className: ExampleClassName,
            data: ExampleData,
            queueName: ExampleQueueName,
            workerName: ExampleWorkerName
        );

        // Requeue with dependencies.
        var secondQueueName = "second-queue";
        var dependencies = new string[] { dependencyJid };
        bool completedSuccessfully = await client.CompleteAndRequeueJobAsync(
            data: ExampleData,
            dependencies: dependencies,
            jid: jid,
            nextQueueName: secondQueueName,
            queueName: firstQueueName,
            workerName: ExampleWorkerName
        );
        Assert.True(completedSuccessfully);

        job = await client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal("depends", job.State);
        Assert.Equal(secondQueueName, job.QueueName);
        Assert.Equal(dependencies, job.Dependencies);

        // Complete the dependency so we can requeue a third time.
        var dependencyJob = await client.PopJobAsync(
            queueName: ExampleQueueName,
            workerName: ExampleWorkerName
        );
        Assert.NotNull(dependencyJob);
        Assert.Equal(dependencyJid, dependencyJob.Jid);

        // Requeue dependency without dependencies or delay.
        completedSuccessfully = await client.CompleteAndRequeueJobAsync(
            data: ExampleData,
            jid: dependencyJid,
            nextQueueName: firstQueueName,
            queueName: ExampleQueueName,
            workerName: ExampleWorkerName
        );
        Assert.True(completedSuccessfully);
        dependencyJob = await client.PopJobAsync(firstQueueName, ExampleWorkerName);
        Assert.NotNull(dependencyJob);
        Assert.Equal(dependencyJid, dependencyJob.Jid);
        completedSuccessfully = await client.CompleteJobAsync(
            data: ExampleData,
            jid: dependencyJid,
            queueName: firstQueueName,
            workerName: ExampleWorkerName
        );
        Assert.True(completedSuccessfully);

        job = await client.PopJobAsync(secondQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);

        // Requeue with delay.
        var thirdQueueName = "third-queue";
        completedSuccessfully = await client.CompleteAndRequeueJobAsync(
            data: ExampleData,
            delay: 300,
            jid: jid,
            nextQueueName: thirdQueueName,
            queueName: secondQueueName,
            workerName: ExampleWorkerName
        );
        Assert.True(completedSuccessfully);

        job = await client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal(thirdQueueName, job.QueueName);
        Assert.Equal("waiting", job.State);
        // Though the job is waiting, it should not be in the queue.
        job = await client.PopJobAsync(thirdQueueName, ExampleWorkerName);
        Assert.Null(job);
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailJobAsync"/> should return true when not
    /// given data and successful.
    /// </summary>
    [Fact]
    public async void FailJobAsync_ReturnsTrueWhenNotGivenDataAndSuccessful()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(
            client,
            queueName: Maybe<string>.Some(ExampleQueueName),
            workerName: Maybe<string>.Some(ExampleWorkerName)
        );
        Job? job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        var failedSuccessfully = await client.FailJobAsync(
            jid,
            ExampleWorkerName,
            ExampleGroup,
            ExampleMessage
        );
        Assert.True(failedSuccessfully);
        job = await client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal("failed", job.State);
        Assert.NotNull(job.Failure);
        Assert.Equal(ExampleWorkerName, job.Failure.WorkerName);
        Assert.Equal(ExampleGroup, job.Failure.Group);
        Assert.Equal(ExampleMessage, job.Failure.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailJobAsync"/> should return true when given
    /// data and successful.
    /// </summary>
    [Fact]
    public async void FailJobAsync_ReturnsTrueWhenGivenDataAndSuccessful()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(
            client,
            queueName: Maybe<string>.Some(ExampleQueueName),
            workerName: Maybe<string>.Some(ExampleWorkerName)
        );
        Job? job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        var failedSuccessfully = await client.FailJobAsync(
            jid,
            ExampleWorkerName,
            ExampleGroup,
            ExampleMessage,
            ExampleUpdatedData
        );
        Assert.True(failedSuccessfully);
        job = await client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal("failed", job.State);
        Assert.Equal(ExampleUpdatedData, job.Data);
        Assert.NotNull(job.Failure);
        Assert.Equal(ExampleWorkerName, job.Failure.WorkerName);
        Assert.Equal(ExampleGroup, job.Failure.Group);
        Assert.Equal(ExampleMessage, job.Failure.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailureGroupsCountsAsync"/> returns the counts
    /// of the various groups of
    /// failures.
    /// </summary>
    [Fact]
    public async void FailureGroupsCountsAsync_ReturnsCountsOfVariousFailureGroups()
    {
        using var client = MakeClientFromConnection(_connection);
        var initialFailedCounts = await client.FailureGroupsCountsAsync();
        Assert.Empty(initialFailedCounts);
        var jid = await PutJobAsync(
            client,
            queueName: Maybe<string>.Some(ExampleQueueName),
            workerName: Maybe<string>.Some(ExampleWorkerName)
        );
        Job? job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        var failedSuccessfully = await client.FailJobAsync(
            jid,
            ExampleWorkerName,
            ExampleGroup,
            ExampleMessage
        );
        Assert.True(failedSuccessfully);
        var failedCounts = await client.FailureGroupsCountsAsync();
        Assert.Equal(
            new Dictionary<string, int> { { ExampleGroup, 1 } },
            failedCounts
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllConfigsAsync "/>should return the
    /// expected default configs.
    /// </summary>
    [Fact]
    public async void GetAllConfigsAsync_ReturnsExpectedDefaultConfigs()
    {
        using var client = MakeClientFromConnection(_connection);
        var configs = await client.GetAllConfigsAsync();

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
    /// <see cref="ReqlessClient.GetCompletedJobsAsync"/> should return an empty
    /// list when there are no completed jobs.
    /// </summary>
    [Fact]
    public async void GetCompletedJobsAsync_ReturnsEmptyListWhenNoSuchJobs()
    {
        using var client = MakeClientFromConnection(_connection);
        var jobs = await client.GetCompletedJobsAsync();
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetCompletedJobsAsync"/> should return expected
    /// jids when there are completed jobs.
    /// </summary>
    [Fact]
    public async void GetCompletedJobsAsync_ReturnsExpectedCompletedJobJids()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(
            client,
            data: Maybe<string>.Some(ExampleData),
            queueName: Maybe<string>.Some(ExampleQueueName),
            workerName: Maybe<string>.Some(ExampleWorkerName)
        );
        Job? job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        bool completedSuccessfully = await client.CompleteJobAsync(
            data: ExampleData,
            jid: jid,
            queueName: ExampleQueueName,
            workerName: ExampleWorkerName
        );
        Assert.True(completedSuccessfully);
        var jobs = await client.GetCompletedJobsAsync();
        Assert.Equivalent(new string[] { jid }, jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobAsync "/>should return null when the job
    /// does not exist.
    /// </summary>
    [Fact]
    public async void GetJobAsync_ReturnsNullWhenJobDoesNotExist()
    {
        using var client = MakeClientFromConnection(_connection);
        var job = await client.GetJobAsync("no-such-jid");
        Assert.Null(job);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobAsync"/>should return the job when the
    /// job exists.
    /// </summary>
    [Fact]
    public async void GetJobAsync_ReturnsTheJobWhenItExists()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(client);
        var job = await client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> should return an empty array
    /// when the jobs do not exist.
    /// </summary>
    [Fact]
    public async void GetJobsAsync_ReturnsEmptyArrayWhenJobsDoNotExist()
    {
        using var client = MakeClientFromConnection(_connection);
        var jobs = await client.GetJobsAsync(
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
    public async void GetJobsAsync_ReturnsTheJobsWhenTheyExist()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(client);
        var otherJid = await PutJobAsync(client);
        var jobs = await client.GetJobsAsync(jid, otherJid);
        Assert.NotNull(jobs);
        Assert.Equal(2, jobs.Length);
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
    public async void GetJobsByStateAsync_ReturnsEmptyListWhenNoSuchJobs()
    {
        using var client = MakeClientFromConnection(_connection);
        var jobs = await client.GetJobsByStateAsync(
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
    public async void GetJobsByStateAsync_ReturnsJobsWhenThereAreMatchingJobs()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(
            client,
            data: Maybe<string>.Some(ExampleData),
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        var jobs = await client.GetJobsByStateAsync("running", ExampleQueueName);
        Assert.Equal(jid, jobs[0]);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueLengthAsync"/> should return zero when
    /// the queue is empty.
    /// </summary>
    [Fact]
    public async void GetQueueLengthAsync_ReturnsZeroWhenQueueIsEmpty()
    {
        using var client = MakeClientFromConnection(_connection);
        var length = await client.GetQueueLengthAsync(ExampleQueueName);
        Assert.Equal(0, length);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueLengthAsync"/> should return expected
    /// length for non-empty queue.
    /// </summary>
    [Fact]
    public async void GetQueueLengthAsync_ReturnsExpectedLengthWhenQueueIsNotEmpty()
    {
        var count = 10;
        using var client = MakeClientFromConnection(_connection);
        for (var i = 0; i < count; i++)
        {
            await PutJobAsync(client);
        }
        var length = await client.GetQueueLengthAsync(ExampleQueueName);
        Assert.Equal(count, length);
    }

    /// <summary>
    /// <see cref="ReqlessClient.HeartbeatJobAsync"/> should return the new
    /// expiration time when not given data and when successful.
    /// </summary>
    [Fact]
    public async void HeartbeatJobAsync_ReturnsTheNewExpirationTimeWithoutData()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(
            client,
            data: Maybe<string>.Some(ExampleData),
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        await Task.Delay(5);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        var newExpiration = await client.HeartbeatJobAsync(
            jid,
            workerName: ExampleWorkerName
        );
        Assert.True(newExpiration > job.Expires);

        var updatedJob = await client.GetJobAsync(job.Jid);
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
    public async void HeartbeatJobAsync_ReturnsTheNewExpirationTimeWithData()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(
            client,
            data: Maybe<string>.Some(ExampleData),
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        await Task.Delay(5);
        var newExpiration = await client.HeartbeatJobAsync(
            data: ExampleUpdatedData,
            jid: jid,
            workerName: ExampleWorkerName
        );
        Assert.True(newExpiration >= job.Expires);
        var updatedJob = await client.GetJobAsync(job.Jid);
        Assert.NotNull(updatedJob);
        Assert.Equal(ExampleUpdatedData, updatedJob.Data);
        Assert.Equal(newExpiration, updatedJob.Expires);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobAsync"/> should return null when there
    /// are no jobs in the queue.
    /// </summary>
    [Fact]
    public async void PopJobAsync_ReturnsNullWhenNoJobsInQueue()
    {
        using var client = MakeClientFromConnection(_connection);
        var job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.Null(job);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobAsync"/> should return a job if there is
    /// a job in the queue.
    /// </summary>
    [Fact]
    public async void PopJobAsync_ReturnsTheJobWhenOneExists()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(
            client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.IsType<Job>(job);
        Assert.Equal(jid, job.Jid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobsAsync"/> should return an empty list
    /// when there are no jobs in the queue.
    /// </summary>
    [Fact]
    public async void PopJobsAsync_ReturnsEmptyListWhenNoJobsInQueue()
    {
        using var client = MakeClientFromConnection(_connection);
        var jobs = await client.PopJobsAsync(ExampleQueueName, ExampleWorkerName, 4);
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobsAsync"/> should return jobs when there
    /// are jobs in the queue.
    /// </summary>
    [Fact]
    public async void PopJobsAsync_ReturnsJobsWhenThereAreJobsInQueue()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(
            client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var otherJid = await PutJobAsync(
            client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var jobs = await client.PopJobsAsync(
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
    public async void PutJobAsync_CanPutAndReceiveJid()
    {
        var dependencyJid = "dependencyJid";
        var dependencies = new string[] { dependencyJid };
        var priority = 0;
        var retries = 5;
        var tags = new string[] { "tags" };
        var throttles = new string[] { "throttles" };

        using var client = MakeClientFromConnection(_connection);
        var dependencyPutJid = await client.PutJobAsync(
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

        var jid = await client.PutJobAsync(
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

        Job? dependency = await client.GetJobAsync(dependencyJid);
        Assert.NotNull(dependency);
        Assert.Equivalent(new string[] { jid }, dependency.Dependents);
        Assert.Equal("waiting", dependency.State);

        Job? subject = await client.GetJobAsync(jid);
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

        await client.CancelJobAsync(jid);
        await client.CancelJobAsync(dependencyJid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveDependencyFromJobAsync"/> should return
    /// true when successful.
    /// </summary>
    [Fact]
    public async void RemoveDependencyFromJobAsync_ReturnsTrueWhenSuccessful()
    {
        using var client = MakeClientFromConnection(_connection);
        var dependsOnJid = await PutJobAsync(client);
        var otherDependsOnJid = await PutJobAsync(client);
        var jid = await PutJobAsync(
            client,
            dependencies: Maybe<string[]>.Some([dependsOnJid, otherDependsOnJid])
        );
        var removedSuccessfully = await client.RemoveDependencyFromJobAsync(
            jid,
            otherDependsOnJid
        );
        Assert.True(removedSuccessfully);
        var job = await client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equal([dependsOnJid], job.Dependencies);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromJobAsync"/> should return the
    /// updated list of tags.
    /// </summary>
    [Fact]
    public async void RemoveTagFromJobAsync_ReturnsTheUpdatedTagsList()
    {
        using var client = MakeClientFromConnection(_connection);
        var initialTag = "initial-tag";
        var otherTag = "other-tag";
        var jid = await PutJobAsync(
            client,
            tags: Maybe<string[]>.Some([initialTag, otherTag])
        );
        var updatedTags = await client.RemoveTagFromJobAsync(jid, otherTag);
        var expectedTags = new string[] { initialTag };
        Assert.Equivalent(expectedTags, updatedTags);
        var job = await client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equivalent(expectedTags, job.Tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromJobAsync"/> should return the
    /// updated list of tags.
    /// </summary>
    [Fact]
    public async void RemoveTagsFromJobAsync_ReturnsTheUpdatedTagsList()
    {
        using var client = MakeClientFromConnection(_connection);
        var initialTag = "initial-tag";
        var otherTag = "other-tag";
        var otherOtherTag = "other-other-tag";
        var jid = await PutJobAsync(
            client,
            tags: Maybe<string[]>.Some([initialTag, otherTag, otherOtherTag])
        );
        var updatedTags = await client.RemoveTagsFromJobAsync(
            jid,
            otherTag,
            otherOtherTag
        );
        var expectedTags = new string[] { initialTag };
        Assert.Equivalent(expectedTags, updatedTags);
        var job = await client.GetJobAsync(jid);
        Assert.NotNull(job);
        Assert.Equivalent(expectedTags, job.Tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should requeue the job with
    /// expected updates.
    /// </summary>
    [Fact]
    public async void RequeueJobAsync_RequeuesAndUpdatesTheJob()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(client);
        var jobBefore = await client.GetJobAsync(jid);
        Assert.NotNull(jobBefore);

        var className = "new-class-name";
        var data = """{"new":true}""";
        var priority = jobBefore.Priority + 1;
        var queueName = "new-queue-name";
        var retries = jobBefore.Retries + 1;
        var tags = new string[] { "tag" };
        var throttles = new string[] { "throttle" };
        var workerName = "new-worker-name";

        var dependencyJid = await PutJobAsync(client);
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

        var jidFromRequeue = await client.RequeueJobAsync(
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

        Job? jobAfter = await client.GetJobAsync(jid);
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

        await client.CancelJobAsync(jid);
        await client.CancelJobAsync(dependencyJid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should return true if the job
    /// has retries remaining and job metadata should be updated to reflect the
    /// retry.
    /// </summary>
    [Fact]
    public async void RetryJobAsync_SchedulesAJobForRetryIfTheJobHasRetriesRemaining()
    {
        using var client = MakeClientFromConnection(_connection);
        var retries = 5;
        var jid = await PutJobAsync(
            client,
            retries: Maybe<int>.Some(retries),
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.IsType<Job>(job);
        Assert.Equal(jid, job.Jid);
        Assert.Equal(retries, job.Retries);
        Assert.Equal(retries, job.Remaining);
        var retrySuccessful = await client.RetryJobAsync(
            jid,
            ExampleQueueName,
            ExampleWorkerName,
            ExampleGroup,
            ExampleMessage
        );
        Assert.True(retrySuccessful);
        Job? retriedJob = await client.GetJobAsync(jid);
        Assert.NotNull(retriedJob);
        Assert.Equal("waiting", retriedJob.State);
        Assert.NotNull(retriedJob.Failure);
        Assert.Equal(ExampleWorkerName, retriedJob.Failure.WorkerName);
        Assert.Equal(ExampleGroup, retriedJob.Failure.Group);
        Assert.Equal(ExampleMessage, retriedJob.Failure.Message);
        Assert.Equal(retries - 1, retriedJob.Remaining);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should return false if the job
    /// has no retries remaining and job metadata should be updated as expected
    /// to reflect the failure.
    /// </summary>
    [Fact]
    public async void RetryJobAsync_DoesNotScheduleAJobForRetryIfTheJobHasNoRetriesRemaining()
    {
        using var client = MakeClientFromConnection(_connection);
        var retries = 0;
        var jid = await PutJobAsync(
            client,
            retries: Maybe<int>.Some(retries),
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.IsType<Job>(job);
        Assert.Equal(jid, job.Jid);
        Assert.Equal(retries, job.Retries);
        Assert.Equal(retries, job.Remaining);
        var retrySuccessful = await client.RetryJobAsync(
            jid,
            ExampleQueueName,
            ExampleWorkerName,
            ExampleGroup,
            ExampleMessage
        );
        Assert.False(retrySuccessful);
        Job? failedJob = await client.GetJobAsync(jid);
        Assert.NotNull(failedJob);
        Assert.Equal("failed", failedJob.State);
        Assert.NotNull(failedJob.Failure);
        Assert.Equal(ExampleWorkerName, failedJob.Failure.WorkerName);
        Assert.Equal(ExampleGroup, failedJob.Failure.Group);
        Assert.Equal(ExampleMessage, failedJob.Failure.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should retry with a delay if a
    /// delay is given.
    /// </summary>
    [Fact]
    public async void RetryJobAsync_SchedulesARetryWithADelayWhenGivenADelay()
    {
        using var client = MakeClientFromConnection(_connection);
        var retries = 5;
        var jid = await PutJobAsync(
            client,
            retries: Maybe<int>.Some(retries),
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.IsType<Job>(job);
        Assert.Equal(jid, job.Jid);
        Assert.Equal(retries, job.Retries);
        Assert.Equal(retries, job.Remaining);
        var retrySuccessful = await client.RetryJobAsync(
            delay: 300,
            group: ExampleGroup,
            jid: jid,
            message: ExampleMessage,
            queueName: ExampleQueueName,
            workerName: ExampleWorkerName
        );
        Assert.True(retrySuccessful);
        Job? failedJob = await client.GetJobAsync(jid);
        Assert.NotNull(failedJob);
        Assert.Equal("scheduled", failedJob.State);
        Assert.NotNull(failedJob.Failure);
        Assert.Equal(ExampleWorkerName, failedJob.Failure.WorkerName);
        Assert.Equal(ExampleGroup, failedJob.Failure.Group);
        Assert.Equal(ExampleMessage, failedJob.Failure.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetJobPriorityAsync"/> should update the job
    /// priority.
    /// </summary>
    [Fact]
    public async void SetJobPriorityAsync_UpdatesTheJobPriority()
    {
        using var client = MakeClientFromConnection(_connection);
        var priority = 0;
        var jid = await PutJobAsync(
            client,
            priority: Maybe<int>.Some(priority)
        );
        var newPriority = 1;
        var updatedSuccessfully = await client.SetJobPriorityAsync(
            jid,
            newPriority
        );
        Assert.True(updatedSuccessfully);
        Job? updatedJob = await client.GetJobAsync(jid);
        Assert.NotNull(updatedJob);
        Assert.Equal(newPriority, updatedJob.Priority);
    }

    /// <summary>
    /// <see cref="ReqlessClient.TimeoutJobAsync"/> should cause the job to time
    /// out.
    /// </summary>
    [Fact]
    public async void TimeoutJobAsync_TimesOutTheJob()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(
            client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        await client.TimeoutJobAsync(jid);
        Job? timedOutJob = await client.GetJobAsync(jid);
        Assert.NotNull(timedOutJob);
        Assert.Equal("stalled", timedOutJob.State);
        Assert.Null(timedOutJob.WorkerName);
    }

    /// <summary>
    /// <see cref="ReqlessClient.TimeoutJobsAsync"/> should cause the jobs to
    /// time out.
    /// </summary>
    [Fact]
    public async void TimeoutJobsAsync_TimesOutTheJobs()
    {
        using var client = MakeClientFromConnection(_connection);
        var jid = await PutJobAsync(
            client,
            queueName: Maybe<string>.Some(ExampleQueueName)
        );
        var otherQueueName = "other-queue-name";
        var otherJid = await PutJobAsync(
            client,
            queueName: Maybe<string>.Some(otherQueueName)
        );
        var job = await client.PopJobAsync(ExampleQueueName, ExampleWorkerName);
        Assert.NotNull(job);
        Assert.Equal(jid, job.Jid);
        var otherJob = await client.PopJobAsync(otherQueueName, ExampleWorkerName);
        Assert.NotNull(otherJob);
        Assert.Equal(otherJid, otherJob.Jid);
        await client.TimeoutJobsAsync(jid, otherJid);

        foreach (var timedOutJid in new string[] { jid, otherJid })
        {
            Job? timedOutJob = await client.GetJobAsync(timedOutJid);
            Assert.NotNull(timedOutJob);
            Assert.Equal("stalled", timedOutJob.State);
            Assert.Null(timedOutJob.WorkerName);
        }
    }

    private static async Task<string> PutJobAsync(
        ReqlessClient client,
        Maybe<string>? className = null,
        Maybe<string>? data = null,
        Maybe<string[]>? dependencies = null,
        Maybe<int>? priority = null,
        Maybe<string>? queueName = null,
        Maybe<int>? retries = null,
        Maybe<string[]>? tags = null,
        Maybe<string>? workerName = null
    )
    {
        var jid = await client.PutJobAsync(
            className: (className ?? Maybe<string>.None).GetOrDefault(ExampleClassName),
            data: (data ?? Maybe<string>.None).GetOrDefault(ExampleData),
            dependencies: (dependencies ?? Maybe<string[]>.None).GetOrDefault([]),
            priority: (priority ?? Maybe<int>.None).GetOrDefault(0),
            retries: (retries ?? Maybe<int>.None).GetOrDefault(5),
            queueName: (queueName ?? Maybe<string>.None).GetOrDefault(ExampleQueueName),
            tags: (tags ?? Maybe<string[]>.None).GetOrDefault([]),
            workerName: (workerName ?? Maybe<string>.None).GetOrDefault(ExampleWorkerName)
        );
        return jid;
    }

    /// <summary>
    /// Make a new <see cref="ReqlessClient"/> from the given connection.
    /// </summary>
    /// <param name="connection">The connection to the Redis server.</param>
    public static ReqlessClient MakeClientFromConnection(
        ConnectionMultiplexer connection
    )
    {
        return new ReqlessClient(new RedisExecutor(connection));
    }
}