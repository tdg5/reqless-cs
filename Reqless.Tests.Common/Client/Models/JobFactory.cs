using Reqless.Client.Models;
using Reqless.Common.Utilities;
using System.Text;
using System.Text.Json;

namespace Reqless.Tests.Common.Client.Models;

/// <summary>
/// Factory for creating <see cref="Job"/> instances and/or JSON for use in
/// tests.
/// </summary>
public static class JobFactory
{
    /// <summary>
    /// Creates a new <see cref="Job"/> instance.
    /// </summary>
    /// <remarks>
    /// For the purposes of this method, for each argument, a given value of
    /// null will get a default value, a value of Maybe{T}.None will cause the
    /// respective property to be omitted from the output, and a value of
    /// Maybe{T}.Some(...) will cause the given value be rendered in the output.
    /// </remarks>
    /// <param name="className">Maybe wrapping the class name of the job.</param>
    /// <param name="data">Maybe wrapping the data of the job.</param>
    /// <param name="dependencies">Maybe wrapping the dependencies of the job.</param>
    /// <param name="dependents">Maybe wrapping the dependents of the job.</param>
    /// <param name="expires">Maybe wrapping the expiration time of the job.</param>
    /// <param name="failure">Maybe wrapping the failure of the job.</param>
    /// <param name="history">Maybe wrapping the history of the job.</param>
    /// <param name="jid">Maybe wrapping the job ID of the job.</param>
    /// <param name="priority">Maybe wrapping the priority of the job.</param>
    /// <param name="queueName">Maybe wrapping the queue name of the job.</param>
    /// <param name="remaining">Maybe wrapping the remaining attempts of the job.</param>
    /// <param name="retries">Maybe wrapping the number of retries of the job.</param>
    /// <param name="spawnedFromJid">Maybe wrapping the spawned from job ID of the job.</param>
    /// <param name="state">Maybe wrapping the state of the job.</param>
    /// <param name="tags">Maybe wrapping the tags of the job.</param>
    /// <param name="throttles">Maybe wrapping the throttles of the job.</param>
    /// <param name="tracked">Maybe wrapping whether the job is tracked.</param>
    /// <param name="workerName">Maybe wrapping the worker name of the job.</param>
    /// <returns>A new <see cref="Job"/> instance.</returns>
    public static Job NewJob(
        Maybe<string>? className = null,
        Maybe<string>? data = null,
        Maybe<string[]>? dependencies = null,
        Maybe<string[]>? dependents = null,
        Maybe<long?>? expires = null,
        Maybe<JobFailure>? failure = null,
        Maybe<JobEvent[]>? history = null,
        Maybe<string>? jid = null,
        Maybe<int>? priority = null,
        Maybe<string>? queueName = null,
        Maybe<int>? remaining = null,
        Maybe<int>? retries = null,
        Maybe<string>? spawnedFromJid = null,
        Maybe<string>? state = null,
        Maybe<string[]>? tags = null,
        Maybe<string[]>? throttles = null,
        Maybe<bool>? tracked = null,
        Maybe<string>? workerName = null)
    {
        return new Job(
            className: (className ?? Maybe<string>.None).GetOrDefault("className"),
            data: (data ?? Maybe<string>.None).GetOrDefault("{}"),
            dependencies: (dependencies ?? Maybe<string[]>.None).GetOrDefault([]),
            dependents: (dependents ?? Maybe<string[]>.None).GetOrDefault([]),
            expires: (expires ?? Maybe<long?>.None).GetOrDefault(GetNow()),
            failure: (failure ?? Maybe<JobFailure>.None).GetOrDefault(
                new JobFailure(
                    group: "group",
                    message: "message",
                    when: GetNow(),
                    workerName: "workerName")),
            history: (history ?? Maybe<JobEvent[]>.None).GetOrDefault([]),
            jid: (jid ?? Maybe<string>.None).GetOrDefault("jid"),
            priority: (priority ?? Maybe<int>.None).GetOrDefault(25),
            queueName: (queueName ?? Maybe<string>.None).GetOrDefault("queueName"),
            remaining: (remaining ?? Maybe<int>.None).GetOrDefault(5),
            retries: (retries ?? Maybe<int>.None).GetOrDefault(6),
            spawnedFromJid: (spawnedFromJid ?? Maybe<string>.None)
                .GetOrDefault("spawnedFromJid"),
            state: (state ?? Maybe<string>.None).GetOrDefault("waiting"),
            tags: (tags ?? Maybe<string[]>.None).GetOrDefault([]),
            throttles: (throttles ?? Maybe<string[]>.None).GetOrDefault([]),
            tracked: (tracked ?? Maybe<bool>.None).GetOrDefault(false),
            workerName: (workerName ?? Maybe<string>.None).GetOrDefault("workerName"));
    }

    /// <summary>
    /// Returns a JSON string representing a <see cref="Job"/>.
    /// </summary>
    /// <remarks>
    /// For the purposes of this method, for each argument, a given value of
    /// null will get a default value, a value of Maybe{T}.None will cause the
    /// respective property to be omitted from the output, and a value of
    /// Maybe{T}.Some(...) will cause the given value be rendered in the output.
    /// </remarks>
    /// <param name="className">Maybe wrapping the class name of the job.</param>
    /// <param name="data">Maybe wrapping the data of the job.</param>
    /// <param name="dependencies">Maybe wrapping the dependencies of the job.</param>
    /// <param name="dependents">Maybe wrapping the dependents of the job.</param>
    /// <param name="expires">Maybe wrapping the expiration time of the job.</param>
    /// <param name="failure">Maybe wrapping the failure of the job.</param>
    /// <param name="history">Maybe wrapping the history of the job.</param>
    /// <param name="jid">Maybe wrapping the job ID of the job.</param>
    /// <param name="priority">Maybe wrapping the priority of the job.</param>
    /// <param name="queueName">Maybe wrapping the queue name of the job.</param>
    /// <param name="remaining">Maybe wrapping the remaining attempts of the job.</param>
    /// <param name="retries">Maybe wrapping the number of retries of the job.</param>
    /// <param name="spawnedFromJid">Maybe wrapping the spawned from job ID of the job.</param>
    /// <param name="state">Maybe wrapping the state of the job.</param>
    /// <param name="tags">Maybe wrapping the tags of the job.</param>
    /// <param name="throttles">Maybe wrapping the throttles of the job.</param>
    /// <param name="tracked">Maybe wrapping whether the job is tracked.</param>
    /// <param name="workerName">Maybe wrapping the worker name of the job.</param>
    /// <returns>A JSON string representing a job.</returns>
    public static string JobJson(
        Maybe<string>? className = null,
        Maybe<string>? data = null,
        Maybe<string[]>? dependencies = null,
        Maybe<string[]>? dependents = null,
        Maybe<long?>? expires = null,
        Maybe<JobFailure>? failure = null,
        Maybe<JobEvent[]>? history = null,
        Maybe<string>? jid = null,
        Maybe<int?>? priority = null,
        Maybe<string>? queueName = null,
        Maybe<int?>? remaining = null,
        Maybe<int?>? retries = null,
        Maybe<string>? spawnedFromJid = null,
        Maybe<string>? state = null,
        Maybe<string[]>? tags = null,
        Maybe<string[]>? throttles = null,
        Maybe<bool?>? tracked = null,
        Maybe<string>? workerName = null)
    {
        static string JsonSerialize<T>(T value) =>
            JsonSerializer.Serialize(value);

        return JobJsonRaw(
            className: (className ?? Maybe.Some("className"))
                .Map(JsonSerialize),
            data: (data ?? Maybe.Some("{}")).Map(JsonSerialize),
            dependencies: (dependencies ?? Maybe<string[]>.Some([]))
                .Map(JsonSerialize),
            dependents: (dependents ?? Maybe<string[]>.Some([]))
                .Map(JsonSerialize),
            expires: (expires ?? Maybe<long?>.Some(GetNow()))
                .Map(value => value?.ToString() ?? "null"),
            failure: (
                    failure ?? Maybe.Some(
                        new JobFailure(
                            group: "group",
                            message: "message",
                            when: GetNow(),
                            workerName: "workerName")))
                .Map(JsonSerialize),
            history: (history ?? Maybe<JobEvent[]>.Some([]))
                .Map(JsonSerialize),
            jid: (jid ?? Maybe.Some("jid")).Map(JsonSerialize),
            priority: (priority ?? Maybe<int?>.Some(25))
                .Map(value => value?.ToString() ?? "null"),
            queueName: (queueName ?? Maybe.Some("queueName"))
                .Map(JsonSerialize),
            remaining: (remaining ?? Maybe<int?>.Some(5))
                .Map(value => value?.ToString() ?? "null"),
            retries: (retries ?? Maybe<int?>.Some(6))
                .Map(value => value?.ToString() ?? "null"),
            spawnedFromJid: (spawnedFromJid ?? Maybe.Some("spawnedFromJid"))
                .Map(JsonSerialize),
            state: (state ?? Maybe.Some("waiting")).Map(JsonSerialize),
            tags: (tags ?? Maybe<string[]>.Some([])).Map(JsonSerialize),
            throttles: (throttles ?? Maybe<string[]>.Some([]))
                .Map(JsonSerialize),
            tracked: (tracked ?? Maybe<bool?>.Some(false))
                .Map(value => value is null ? "null" : value == true ? "true" : "false"),
            workerName: (workerName ?? Maybe.Some("workerName"))
                .Map(JsonSerialize));
    }

    /// <summary>
    /// Returns a JSON string representing a <see cref="Job"/>. All arguments
    /// are expected to have been serialized to JSON format already.
    /// </summary>
    /// <remarks>
    /// For the purposes of this method, for each argument, a given value of
    /// null will get a default value, a value of Maybe{T}.None will cause the
    /// respective property to be omitted from the output, and a value of
    /// Maybe{T}.Some(...) will cause the given value be rendered in the output.
    /// </remarks>
    /// <param name="className">Maybe wrapping the class name of the job.</param>
    /// <param name="data">Maybe wrapping the data of the job.</param>
    /// <param name="dependencies">Maybe wrapping the dependencies of the job.</param>
    /// <param name="dependents">Maybe wrapping the dependents of the job.</param>
    /// <param name="expires">Maybe wrapping the expiration time of the job.</param>
    /// <param name="failure">Maybe wrapping the failure of the job.</param>
    /// <param name="history">Maybe wrapping the history of the job.</param>
    /// <param name="jid">Maybe wrapping the job ID of the job.</param>
    /// <param name="priority">Maybe wrapping the priority of the job.</param>
    /// <param name="queueName">Maybe wrapping the queue name of the job.</param>
    /// <param name="remaining">Maybe wrapping the remaining attempts of the job.</param>
    /// <param name="retries">Maybe wrapping the number of retries of the job.</param>
    /// <param name="spawnedFromJid">Maybe wrapping the spawned from job ID of the job.</param>
    /// <param name="state">Maybe wrapping the state of the job.</param>
    /// <param name="tags">Maybe wrapping the tags of the job.</param>
    /// <param name="throttles">Maybe wrapping the throttles of the job.</param>
    /// <param name="tracked">Maybe wrapping whether the job is tracked.</param>
    /// <param name="unknown">Maybe wrapping an unknown property of the job.</param>
    /// <param name="workerName">Maybe wrapping the worker name of the job.</param>
    /// <returns>A JSON string representing a job.</returns>
    public static string JobJsonRaw(
        Maybe<string>? className = null,
        Maybe<string>? data = null,
        Maybe<string>? dependencies = null,
        Maybe<string>? dependents = null,
        Maybe<string>? expires = null,
        Maybe<string>? failure = null,
        Maybe<string>? history = null,
        Maybe<string>? jid = null,
        Maybe<string>? priority = null,
        Maybe<string>? queueName = null,
        Maybe<string>? remaining = null,
        Maybe<string>? retries = null,
        Maybe<string>? spawnedFromJid = null,
        Maybe<string>? state = null,
        Maybe<string>? tags = null,
        Maybe<string>? throttles = null,
        Maybe<string>? tracked = null,
        Maybe<string>? unknown = null,
        Maybe<string>? workerName = null)
    {
        var now = GetNow();
        var classNameMaybe = className ?? Maybe.Some("\"className\"");
        var dataMaybe = data ?? Maybe.Some("\"{}\"");
        var dependenciesMaybe = dependencies ?? Maybe.Some("[]");
        var dependentsMaybe = dependents ?? Maybe.Some("[]");
        var expiresMaybe = expires ?? Maybe.Some((now + 60000).ToString());
        var failureMaybe = failure ?? Maybe.Some(
            JsonSerializer.Serialize(
                new JobFailure(
                    group: "group",
                    message: "message",
                    when: GetNow(),
                    workerName: "workerName")));
        var historyMaybe = history ?? Maybe.Some("[]");
        var jidMaybe = jid ?? Maybe.Some("\"jid\"");
        var priorityMaybe = priority ?? Maybe.Some("25");
        var queueNameMaybe = queueName ?? Maybe.Some("\"queueName\"");
        var remainingMaybe = remaining ?? Maybe.Some("5");
        var retriesMaybe = retries ?? Maybe.Some("6");
        var spawnedFromJidMaybe = spawnedFromJid ?? Maybe.Some("\"spawnedFromJid\"");
        var stateMaybe = state ?? Maybe.Some("\"state\"");
        var tagsMaybe = tags ?? Maybe.Some("[]");
        var throttlesMaybe = throttles ?? Maybe.Some("[]");
        var trackedMaybe = tracked ?? Maybe.Some("false");
        var unknownMaybe = unknown ?? Maybe<string>.None;
        var workerNameMaybe = workerName ?? Maybe.Some("\"workerName\"");

        var json = new StringBuilder();
        json.Append('{');
        if (dataMaybe.HasValue)
        {
            var dataValueJson = dataMaybe.GetOrDefault("{}");
            json.Append($"\"data\":{dataValueJson},");
        }

        if (dependenciesMaybe.HasValue)
        {
            var dependenciesValueJson = dependenciesMaybe.GetOrDefault("[]");
            json.Append($"\"dependencies\":{dependenciesValueJson},");
        }

        if (dependentsMaybe.HasValue)
        {
            var dependentsValueJson = dependentsMaybe.GetOrDefault("[]");
            json.Append($"\"dependents\":{dependentsValueJson},");
        }

        if (expiresMaybe.HasValue)
        {
            var expiresValueJson = expiresMaybe.GetOrDefault("12345");
            json.Append($"\"expires\":{expiresValueJson},");
        }

        if (failureMaybe.HasValue)
        {
            var failureValueJson = failureMaybe.GetOrDefault("null");
            json.Append($"\"failure\":{failureValueJson},");
        }

        if (historyMaybe.HasValue)
        {
            var historyValueJson = historyMaybe.GetOrDefault("[]");
            json.Append($"\"history\":{historyValueJson},");
        }

        if (jidMaybe.HasValue)
        {
            var jidValueJson = jidMaybe.GetOrDefault("\"jid\"");
            json.Append($"\"jid\":{jidValueJson},");
        }

        if (classNameMaybe.HasValue)
        {
            var classNameValueJson = classNameMaybe.GetOrDefault("\"className\"");
            json.Append($"\"klass\":{classNameValueJson},");
        }

        if (priorityMaybe.HasValue)
        {
            var priorityValueJson = priorityMaybe.GetOrDefault("12345");
            json.Append($"\"priority\":{priorityValueJson},");
        }

        if (queueNameMaybe.HasValue)
        {
            var queueNameValueJson = queueNameMaybe.GetOrDefault("\"queueName\"");
            json.Append($"\"queue\":{queueNameValueJson},");
        }

        if (remainingMaybe.HasValue)
        {
            var remainingValueJson = remainingMaybe.GetOrDefault("6");
            json.Append($"\"remaining\":{remainingValueJson},");
        }

        if (retriesMaybe.HasValue)
        {
            var retriesValueJson = retriesMaybe.GetOrDefault("5");
            json.Append($"\"retries\":{retriesValueJson},");
        }

        if (spawnedFromJidMaybe.HasValue)
        {
            var spawnedFromJidValueJson = spawnedFromJidMaybe.GetOrDefault("\"spawnedFromJid\"");
            json.Append($"\"spawned_from_jid\":{spawnedFromJidValueJson},");
        }

        if (stateMaybe.HasValue)
        {
            var stateValueJson = stateMaybe.GetOrDefault("\"running\"");
            json.Append($"\"state\":{stateValueJson},");
        }

        if (tagsMaybe.HasValue)
        {
            var tagsJson = tagsMaybe.GetOrDefault("[]");
            json.Append($"\"tags\":{tagsJson},");
        }

        if (throttlesMaybe.HasValue)
        {
            var throttlesJson = throttlesMaybe.GetOrDefault("[]");
            json.Append($"\"throttles\":{throttlesJson},");
        }

        if (trackedMaybe.HasValue)
        {
            var trackedValueJson = trackedMaybe.GetOrDefault("false");
            json.Append($"\"tracked\":{trackedValueJson},");
        }

        if (unknownMaybe.HasValue)
        {
            var unknownValueJson = unknownMaybe.GetOrDefault("null");
            json.Append($"\"unknown\":{unknownValueJson},");
        }

        if (workerNameMaybe.HasValue)
        {
            var workerNameValueJson = workerNameMaybe.GetOrDefault("\"workerName\"");
            json.Append($"\"worker\":{workerNameValueJson},");
        }

        json.Remove(json.Length - 1, 1);
        json.Append('}');
        return json.ToString();
    }

    private static long GetNow()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
