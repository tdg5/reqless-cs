using System.Text.Json;

namespace Reqless.Client.Serialization;

internal static class JsonConverterHelper
{
    /// <summary>
    /// Try to consume a degenerate object (an object with no properties) from
    /// the JSON reader. Redis' cjson can't distinguish between an empty array
    /// and an empty object, so this method is used to handle that case.
    /// </summary>
    /// <param name="propertyName">The name of the property being consumed.</param>
    /// <param name="expectedType">The expected type of the property. Used for
    /// error messages.</param>
    /// <param name="reader">The JSON reader to consume the object from.</param>
    /// <returns>True if a degenerate object was consumed, false otherwise.</returns>
    internal static bool TryConsumeDegenerateObject(
        string propertyName,
        string expectedType,
        ref Utf8JsonReader reader
    )
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            return false;
        }

        var readerClone = reader;
        readerClone.Read();
        if (readerClone.TokenType == JsonTokenType.EndObject)
        {
            reader.Read();
            return true;
        }

        var unexpectedObject = JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader);
        // We can forgive null here because the reader will throw if the object
        // isn't a dictionary.
        var propertyCount = unexpectedObject!.Count;
        throw new JsonException(
            $"Expected '{propertyName}' to be {expectedType} or empty object but encountered object with {propertyCount} properties."
        );
    }
}