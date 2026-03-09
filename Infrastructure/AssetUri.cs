using System;
using System.Reflection;

namespace ProjectPlan.Infrastructure;

public static class AssetUri
{
    public static Uri For(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            throw new ArgumentException("Asset relative path is required", nameof(relativePath));

        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        if (string.IsNullOrWhiteSpace(assemblyName))
            throw new InvalidOperationException("Unable to resolve executing assembly name");

        // Avalonia resource URI: avares://<assembly-name>/<path>
        return new Uri($"avares://{assemblyName}/{relativePath.TrimStart('/')}");
    }
}
