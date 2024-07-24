using Reqless.Models;
using Reqless.Models.JobEvents;
using System.Text.Json;
using System.Text;

namespace Reqless.Tests.TestHelpers.Factories;

/// <summary>
/// Factory for creating <see cref="RecurringJob"/> instances and/or JSON for
/// use in tests.
/// </summary>
public static class RecurringJobFactory
{
    /// <summary>
    /// Creates a new <see cref="RecurringJob"/> instance.
    /// </summary>
    /// <remarks>
    /// For the purposes of this method, for each argument, a given value of
    /// null will get a default value, a value of Maybe{T}.None will cause the
    /// respective property to be omitted from the output, and a value of
    /// Maybe{T}.Some(...) will cause the given value be rendered in the output.
    /// </remarks>
    /// <param name="className">Maybe wrapping the class name of the recurring
    /// job.</param>
    /// <param name="count">Maybe wrapping the count of the recurring job.</param>
    /// <param name="data">Maybe wrapping the data of the recurring job.</param>
    /// <param name="intervalSeconds">Maybe wrapping the interval seconds of the
    /// recurring job.</param>
    /// <param name="jid">Maybe wrapping the job ID of the recurring
    /// job.</param>
    /// <param name="maximumBacklog">Maybe wrapping the maximum backlog of the
    /// recurring job.</param>
    /// <param name="priority">Maybe wrapping the priority of the recurring
    /// job.</param>
    /// <param name="queueName">Maybe wrapping the queue name of the recurring
    /// job.</param>
    /// <param name="retries">Maybe wrapping the number of retries of the
    /// recurring job.</param>
    /// <param name="state">Maybe wrapping the state of the recurring
    /// job.</param>
    /// <param name="tags">Maybe wrapping the tags of the recurring job.</param>
    /// <param name="throttles">Maybe wrapping the throttles of the recurring
    /// job.</param>
    public static RecurringJob NewRecurringJob(
        Maybe<string>? className = null,
        Maybe<int>? count = null,
        Maybe<string>? data = null,
        Maybe<int>? intervalSeconds = null,
        Maybe<string>? jid = null,
        Maybe<int>? maximumBacklog = null,
        Maybe<int>? priority = null,
        Maybe<string>? queueName = null,
        Maybe<int>? retries = null,
        Maybe<string>? state = null,
        Maybe<string[]>? tags = null,
        Maybe<string[]>? throttles = null
    )
    {
        return new RecurringJob(
            className: (className ?? Maybe<string>.None).GetOrDefault("className"),
            count: (count ?? Maybe<int>.None).GetOrDefault(0),
            data: (data ?? Maybe<string>.None).GetOrDefault("{}"),
            intervalSeconds: (intervalSeconds ?? Maybe<int>.None).GetOrDefault(60),
            jid: (jid ?? Maybe<string>.None).GetOrDefault("jid"),
            maximumBacklog: (maximumBacklog ?? Maybe<int>.None).GetOrDefault(10),
            priority: (priority ?? Maybe<int>.None).GetOrDefault(25),
            queueName: (queueName ?? Maybe<string>.None).GetOrDefault("queueName"),
            retries: (retries ?? Maybe<int>.None).GetOrDefault(6),
            state: (state ?? Maybe<string>.None).GetOrDefault("waiting"),
            tags: (tags ?? Maybe<string[]>.None).GetOrDefault([]),
            throttles: (throttles ?? Maybe<string[]>.None).GetOrDefault([])
        );
    }

    /// <summary>
    /// Returns a JSON string representing a <see cref="RecurringJob"/>.
    /// </summary>
    /// <remarks>
    /// For the purposes of this method, for each argument, a given value of
    /// null will get a default value, a value of Maybe{T}.None will cause the
    /// respective property to be omitted from the output, and a value of
    /// Maybe{T}.Some(...) will cause the given value be rendered in the output.
    /// </remarks>
    /// <param name="className">Maybe wrapping the class name of the recurring
    /// job.</param>
    /// <param name="count">Maybe wrapping the count of the recurring job.</param>
    /// <param name="data">Maybe wrapping the data of the recurring job.</param>
    /// <param name="intervalSeconds">Maybe wrapping the interval seconds of the
    /// recurring job.</param>
    /// <param name="jid">Maybe wrapping the job ID of the recurring
    /// job.</param>
    /// <param name="maximumBacklog">Maybe wrapping the maximum backlog of the
    /// recurring job.</param>
    /// <param name="priority">Maybe wrapping the priority of the recurring
    /// job.</param>
    /// <param name="queueName">Maybe wrapping the queue name of the recurring
    /// job.</param>
    /// <param name="retries">Maybe wrapping the number of retries of the
    /// recurring job.</param>
    /// <param name="state">Maybe wrapping the state of the recurring
    /// job.</param>
    /// <param name="tags">Maybe wrapping the tags of the recurring job.</param>
    /// <param name="throttles">Maybe wrapping the throttles of the recurring
    /// job.</param>
    /// <returns>A JSON string representing a <see
    /// cref="RecurringJob"/>.</returns>
    public static string RecurringJobJson(
        Maybe<string?>? className = null,
        Maybe<int?>? count = null,
        Maybe<string?>? data = null,
        Maybe<int?>? intervalSeconds = null,
        Maybe<string?>? jid = null,
        Maybe<int?>? maximumBacklog = null,
        Maybe<int?>? priority = null,
        Maybe<string?>? queueName = null,
        Maybe<int?>? retries = null,
        Maybe<string?>? state = null,
        Maybe<string[]?>? tags = null,
        Maybe<string[]?>? throttles = null
    )
    {
        static string jsonSerialize<T>(T value) =>
            JsonSerializer.Serialize(value);

        return RecurringJobJsonRaw(
            className: (className ?? Maybe<string?>.Some("className"))
                .Map(jsonSerialize),

            count: (count ?? Maybe<int?>.Some(0))
                .Map(value => value?.ToString() ?? "null"),

            data: (data ?? Maybe<string?>.Some("{}")).Map(jsonSerialize),

            intervalSeconds: (intervalSeconds ?? Maybe<int?>.Some(60))
                .Map(value => value?.ToString() ?? "null"),

            jid: (jid ?? Maybe<string?>.Some("jid")).Map(jsonSerialize),

            maximumBacklog: (maximumBacklog ?? Maybe<int?>.Some(10))
                .Map(value => value?.ToString() ?? "null"),

            priority: (priority ?? Maybe<int?>.Some(25))
                .Map(value => value?.ToString() ?? "null"),

            queueName: (queueName ?? Maybe<string?>.Some("queueName"))
                .Map(jsonSerialize),

            retries: (retries ?? Maybe<int?>.Some(6))
                .Map(value => value?.ToString() ?? "null"),

            state: (state ?? Maybe<string?>.Some("waiting")).Map(jsonSerialize),

            tags: (tags ?? Maybe<string[]?>.Some([])).Map(jsonSerialize),

            throttles: (throttles ?? Maybe<string[]?>.Some([]))
                .Map(jsonSerialize)
        );
    }

    /// <summary>
    /// Returns a JSON string representing a <see cref="RecurringJob"/>. All
    /// arguments are expected to have been serialized to JSON format already.
    /// </summary>
    /// <remarks>
    /// For the purposes of this method, for each argument, a given value of
    /// null will get a default value, a value of Maybe{T}.None will cause the
    /// respective property to be omitted from the output, and a value of
    /// Maybe{T}.Some(...) will cause the given value be rendered in the output.
    /// </remarks>
    /// <param name="className">Maybe wrapping the class name of the job.</param>
    /// <param name="count">Maybe wrapping the count of the recurring job.</param>
    /// <param name="data">Maybe wrapping the data of the job.</param>
    /// <param name="intervalSeconds">Maybe wrapping the interval seconds of the
    /// recurring job.</param>
    /// <param name="jid">Maybe wrapping the job ID of the job.</param>
    /// <param name="maximumBacklog">Maybe wrapping the maximum backlog of the
    /// recurring job.</param>
    /// <param name="priority">Maybe wrapping the priority of the job.</param>
    /// <param name="queueName">Maybe wrapping the queue name of the job.</param>
    /// <param name="retries">Maybe wrapping the number of retries of the job.</param>
    /// <param name="state">Maybe wrapping the state of the job.</param>
    /// <param name="tags">Maybe wrapping the tags of the job.</param>
    /// <param name="throttles">Maybe wrapping the throttles of the job.</param>
    /// <param name="unknown">Maybe wrapping an unknown property of the job.</param>
    public static string RecurringJobJsonRaw(
        Maybe<string>? className = null,
        Maybe<string>? count = null,
        Maybe<string>? data = null,
        Maybe<string>? intervalSeconds = null,
        Maybe<string>? jid = null,
        Maybe<string>? maximumBacklog = null,
        Maybe<string>? priority = null,
        Maybe<string>? queueName = null,
        Maybe<string>? retries = null,
        Maybe<string>? state = null,
        Maybe<string>? tags = null,
        Maybe<string>? throttles = null,
        Maybe<string>? unknown = null
    )
    {
        var classNameMaybe = className ?? Maybe<string>.Some("\"className\"");
        var countMaybe = count ?? Maybe<string>.Some("0");
        var dataMaybe = data ?? Maybe<string>.Some("\"{}\"");
        var intervalSecondsMaybe = intervalSeconds ?? Maybe<string>.Some("60");
        var jidMaybe = jid ?? Maybe<string>.Some("\"jid\"");
        var maximumBacklogMaybe = maximumBacklog ?? Maybe<string>.Some("10");
        var priorityMaybe = priority ?? Maybe<string>.Some("25");
        var queueNameMaybe = queueName ?? Maybe<string>.Some("\"queueName\"");
        var retriesMaybe = retries ?? Maybe<string>.Some("6");
        var stateMaybe = state ?? Maybe<string>.Some("\"state\"");
        var tagsMaybe = tags ?? Maybe<string>.Some("[]");
        var throttlesMaybe = throttles ?? Maybe<string>.Some("[]");
        var unknownMaybe = unknown ?? Maybe<string>.None;

        var json = new StringBuilder();
        json.Append('{');
        if (maximumBacklogMaybe.HasValue)
        {
            var maximumBacklogValueJson = maximumBacklogMaybe.GetOrDefault("10");
            json.Append($"\"backlog\":{maximumBacklogValueJson},");
        }
        if (countMaybe.HasValue)
        {
            var countValueJson = countMaybe.GetOrDefault("0");
            json.Append($"\"count\":{countValueJson},");
        }
        if (dataMaybe.HasValue)
        {
            var dataValueJson = dataMaybe.GetOrDefault("{}");
            json.Append($"\"data\":{dataValueJson},");
        }
        if (intervalSecondsMaybe.HasValue)
        {
            var intervalSecondsValueJson = intervalSecondsMaybe.GetOrDefault("60");
            json.Append($"\"interval\":{intervalSecondsValueJson},");
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
        if (retriesMaybe.HasValue)
        {
            var retriesValueJson = retriesMaybe.GetOrDefault("5");
            json.Append($"\"retries\":{retriesValueJson},");
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
        if (unknownMaybe.HasValue)
        {
            var unknownValueJson = unknownMaybe.GetOrDefault("null");
            json.Append($"\"unknown\":{unknownValueJson},");
        }
        json.Remove(json.Length - 1, 1);
        json.Append('}');
        return json.ToString();
    }
}