# reqless-cs

`reqless` is the successor to [`qless`](https://github.com/seomoz/qless), and
`reqless-cs` provides `reqless` client bindings for C#.

`reqless`, like `qless` before it, is a powerful job queueing system based on
remote dictionary servers (like `redis` and `valkey`) inspired by
[resque](https://github.com/defunkt/resque#readme), but built on a collection
of Lua scripts, maintained in the
[qless-core](https://github.com/tdg5/qless-core) repo.

## Philosophy and Nomenclature

A `job` is a unit of work identified by a job id or `jid`. A `queue` can
contain several jobs that are scheduled to be run at a certain time, several
jobs that are waiting to run, and jobs that are currently running. A `worker`
is a process on a host, identified uniquely, that asks for jobs from the
queue, performs some process associated with that job, and then marks it as
complete. When it's completed, it can be put into another queue.

Jobs can only be in one queue at a time. That queue is whatever queue they
were last put in. So if a worker is working on a job, and you move it, the
worker's request to complete the job will be ignored.

A job can be `canceled`, which means it disappears into the ether, and we'll
never pay it any mind every again. A job can be `dropped`, which is when a
worker fails to heartbeat or complete the job in a timely fashion, or a job
can be `failed`, which is when a host recognizes some systematically
problematic state about the job. A worker should only fail a job if the error
is likely not a transient one; otherwise, that worker should just drop it and
let the system reclaim it.

## Features

1. __Jobs don't get dropped on the floor__ -- Sometimes workers drop jobs.
    `qless` automatically picks them back up and gives them to another worker
1. __Tagging / Tracking__ -- Some jobs are more interesting than others. Track
    those jobs to get updates on their progress. Tag jobs with meaningful
    identifiers to find them quickly in the UI.
1. __Job Dependencies__ -- One job might need to wait for another job to
    complete
1. __Stats__ -- `qless` automatically keeps statistics about how long jobs wait
    to be processed and how long they take to be processed. Currently, we keep
    track of the count, mean, standard deviation, and a histogram of these
    times.
1. __Job data is stored temporarily__ -- Job info sticks around for a
    configurable amount of time so you can still look back on a job's history,
    data, etc.
1. __Priority__ -- Jobs with the same priority get popped in the order they
    were inserted; a higher priority means that it gets popped faster
1. __Retry logic__ -- Every job has a number of retries associated with it,
    which are renewed when it is put into a new queue or completed. If a job
    is repeatedly dropped, then it is presumed to be problematic, and is
    automatically failed.
1. __Web App__ -- With the advent of a Ruby client, there is a Sinatra-based
    web app that gives you control over certain operational issues
1. __Scheduled Work__ -- Until a job waits for a specified delay (defaults to
    0), jobs cannot be popped by workers
1. __Recurring Jobs__ -- Scheduling's all well and good, but we also support
    jobs that need to recur periodically.
1. __Notifications__ -- Tracked jobs emit events on pubsub channels as they get
    completed, failed, put, popped, etc. Use these events to get notified of
    progress on jobs you're interested in.

Interest piqued? Then read on!

## Installation

Install with NuGet:

```
# TODO
```

Alternatively, install reqless-cs from source by checking it out from github,
and checking out the qless-core submodule:

```bash
git clone git://github.com/tdg5/reqless-cs.git
cd reqless-cs
# qless-core is a submodule
git submodule init
git submodule update
# TODO
```

## Business Time!

You've read this far -- you probably want to write some code now and turn them
into jobs. Jobs are described essentially by two pieces of information -- a
`class` and `data`. The class should have static methods that know how to
process this type of job depending on the queue it's in. For those thrown for
a loop by this example, it's in reference to a
[South Park](http://en.wikipedia.org/wiki/Gnomes_%28South_Park%29#Plot)
episode where a group of enterprising gnomes set on world domination through
three steps: 1) collect underpants, 2) ? 3) profit!

```csharp
// TODO
```

Jobs have user data associated with them that can be modified as it goes
through a pipeline. In general, user data is accessible through `job.data`. It
is generally handled as a JSON string so that each job class may parse and
handle the user data in whatever way is appropriate for that job class.  For
example, you might update the data like so...

```csharp
// TODO
```

Great! With all this in place, let's put them in the queue so that they can
get run

```csharp
// TODO
```

Now, reference a queue, and start putting your gnomes to work:

```csharp
// TODO
```

Alternatively, if the job class is not importable from where you're adding
jobs, you can use the full path of the job class as a string:

```csharp
// TODO
```

## Running

TODO

## Internals and Additional Features

While in many cases the above is sufficient, there are also many cases where
you may need something more. Hopefully after this section many of your
questions will be answered.

### Priority

Jobs can optionally have priority associated with them. Jobs of equal priority
are popped in the order in which they were put in a queue. The higher the
priority, the sooner it will be processed. If, for example, you get a new job
to collect some really valuable underpants:

```csharp
// TODO
```

You can also adjust a job's priority while it's waiting:

```csharp
// TODO
```

### Scheduled Jobs

Jobs can also be scheduled for the future with a delay (in seconds). If for
example, you just learned of an underpants heist opportunity, but you have to
wait until later:

```csharp
// TODO
```

It's worth noting that it's not guaranteed that this job will run at that time.
It merely means that this job will only be considered valid after the delay
has passed, at which point it will be subject to the normal constraints. If
you want it to be processed very soon after the delay expires, you could also
boost its priority:

```csharp
// TODO
```

### Recurring Jobs

Whether it's nightly maintenance, or weekly customer updates, you can have a
job of a certain configuration set to recur. Recurring jobs still support
priority, and tagging, and are attached to a queue. Let's say, for example, I
need some global maintenance to run, and I don't care what machine runs it, so
long as someone does:

```csharp
// TODO
```

That will spawn a job right now, but it's possible you'd like to have it recur,
but maybe the first job should wait a little bit:

```csharp
// TODO
```

You can always update the tags, priority and even the interval of a recurring job:

```csharp
// TODO
```

These attributes aren't attached to the recurring jobs, per se, but it's used
as the template for the job that it creates. In the case where more than one
interval passes before a worker tries to pop the job, __more than one job is
created__. The thinking is that while it's
completely client-managed, the state should not be dependent on how often
workers are trying to pop jobs.

```csharp
// TODO
```

### Configuration Options

You can get and set global (in the context of the same remote dictionary
server instance) configuration to change the behavior for heartbeating, and so
forth. There aren't a tremendous number of configuration options, but an
important one is how long job data is kept around. Job data is expired after it
has been completed for `jobs-history` seconds, but is limited to the last
`jobs-history-count` completed jobs. These default to 50k jobs, and 30 days,
but depending on volume, your needs may change. To only keep the last 500 jobs
for up to 7 days:

```csharp
// TODO
```

### Tagging / Tracking

In `qless`, "tracking" means flagging a job as important. Tracked jobs have a
tab reserved for them in the web interface, and they also emit events that can
be subscribed to as they make progress (more on that below). You can flag a job
from the web interface, or the corresponding code:

```csharp
// TODO
```

Jobs can be tagged with strings which are indexed for quick searches. For
example, jobs might be associated with customer accounts, or some other key
that makes sense for your project.

```csharp
// TODO
```

This makes them searchable in the web interface, or from code:

```csharp
// TODO
```

You can add or remove tags at will, too:

```csharp
// TODO
```

### Job Dependencies

Jobs can be made dependent on the completion of another job. For example, if
you need to buy eggs, and buy a pan before making an omelet, you could say:

```csharp
// TODO
```

That way, the job to make the omelet can't be performed until the pan and eggs
purchases have been completed.

### Notifications

Tracked jobs emit events on specific pubsub channels as things happen to them.
Whether it's getting popped off of a queue, completed by a worker, etc. The
jist of it goes like this, though:

```csharp
// TODO
```

If you're interested in, say, getting growl or campfire notifications, you
should check out the `qless-growl` and `qless-campfire` ruby gems.

### Retries

Workers sometimes die. That's an unfortunate reality of life. We try to
mitigate the effects of this by insisting that workers heartbeat their jobs to
ensure that they do not get dropped. That said, `qless` server will
automatically requeue jobs that do get "stalled" up to the provided number of
retries (default is 5). Since underpants profit can sometimes go awry, maybe
you want to retry a particular heist several times:

```csharp
// TODO
```

### Pop

A client pops one or more jobs from a queue:

```csharp
// TODO
```

### Heartbeating

Each job object has a notion of when you must either check in with a heartbeat
or turn it in as completed. You can get the absolute time until it expires, or
how long you have left:

```csharp
// TODO
```

If your lease on the job will expire before you have a chance to complete it,
then you should heartbeat it to make sure that no other worker gets access to
it. Or, if you are done, you should complete it so that the job can move on:

```csharp
// TODO
```

### Stats

One of the selling points of `qless` is that it keeps stats for you about your
underpants hijinks. It tracks the average wait time, number of jobs that have
waited in a queue, failures, retries, and average running time. It also keeps
histograms for the number of jobs that have waited _x_ time, and the number
that took _x_ time to run.

Frankly, these are best viewed using the web app.

### Lua

`qless` is a set of client language bindings, but the majority of the work is
done in a collection of Lua scripts that comprise the
[core](https://github.com/tdg5/qless-core) functionality. These scripts run
on `redis` and `valkey` 7.0+ server atomically and allow for portability with
the same functionality guarantees. Consult the documentation for `qless-core`
to learn more about its internals.

### Web App

`qless` also comes with a web app for administrative tasks, like keeping tabs
on the progress of jobs, tracking specific jobs, retrying failed jobs, etc.
It's available in the [`qless`](https://github.com/tdg5/qless) library as a
mountable [`Sinatra`](http://www.sinatrarb.com/) app. The web app is language
agnostic and was one of the major desires out of this project, so you should
consider using it even if you're not planning on using the Ruby client.

The web app is also available as a Docker container,
[`tdg5/reqless-ui`](https://hub.docker.com/repository/docker/tdg5/reqless-ui/general),
with source code available at
[`tdg5/reqless-ui-docker`](https://github.com/tdg5/reqless-ui-docker).
