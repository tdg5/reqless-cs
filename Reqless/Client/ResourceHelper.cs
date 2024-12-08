using System.Reflection;

namespace Reqless.Client;

/// <summary>
/// Helper class for reading embedded resources, in particular the reqless-core
/// lua scripts.
/// </summary>
public static class ResourceHelper
{
    /// <summary>
    /// Reads the text content of an embedded resource.
    /// </summary>
    /// <param name="resourcePath">
    /// The path to the resource, in the format
    /// "folder/folder/file.ext", even on Windows, since the path doesn't refer
    /// to a real file path.
    /// </param>
    /// <returns>The text content of the resource.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the resource is not
    /// found or inaccessible to the calling assembly.</exception>
    public static string ReadTextResource(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var fullResourceName = $"{nameof(Reqless)}.{nameof(Client)}.{resourcePath.Replace("/", ".")}";
        var isKnownResource = assembly.GetManifestResourceNames().Any(
            resourceName => resourceName == fullResourceName);

        if (!isKnownResource)
        {
            throw new FileNotFoundException($"Resource {fullResourceName} not found.");
        }

        // GetManifestResourceStream returns null "if a private resource in
        // another assembly is accessed and the caller does not have
        // [permissions]...".  However, since the code above ensures that the
        // requested resource is part of the Reqless assembly, null should not
        // be returned and thus can be forgiven.
        using Stream stream = assembly.GetManifestResourceStream(fullResourceName)!;
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
