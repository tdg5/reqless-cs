using Reqless.Client.Serialization;
using Reqless.Common.Validation;
using System.Text.Json.Serialization;

namespace Reqless.Client.Models;

/// <summary>
/// Data class representing a response containing a list of jids and the total
/// number of jobs in the respective collection.
/// </summary>
[JsonConverter(typeof(JidsResultJsonConverter))]
public class JidsResult
{
    /// <summary>
    /// A  list of job IDs that are in the collection, possibly incomplete.
    /// </summary>
    public string[] Jids;

    /// <summary>
    /// The total number of jobs in the collection.
    /// </summary>
    public int Total;

    /// <summary>
    /// Initializes a new instance of the <see cref="JidsResult"/>.
    /// </summary>
    /// <param name="total">The total number of jobs in the collection.</param>
    /// <param name="jids">A list of job IDs that are in the collection,
    /// possibly incomplete.</param>
    public JidsResult(string[] jids, int total)
    {
        ArgumentValidation.ThrowIfAnyNullOrWhitespace(jids, nameof(jids));

        Jids = jids;
        Total = total;
    }
}
