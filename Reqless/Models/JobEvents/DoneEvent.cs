namespace Reqless.Models.JobEvents;

/// <summary>
/// Data object representing the event that occurs when a job is completed.
/// </summary>
public class DoneEvent(long when) : JobEvent("done", when)
{
}