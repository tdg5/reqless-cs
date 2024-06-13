namespace Reqless.Models.JobEvents;

/// <summary>
/// Data object representing the event that occurs when a job times out.
/// </summary>
public class TimedOutEvent(long when) : JobEvent("timed-out", when)
{
}