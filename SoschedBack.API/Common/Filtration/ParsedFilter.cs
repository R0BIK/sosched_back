namespace SoschedBack.Common.Filtration;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a parsed set of filters from query string.
/// Behaves like a dictionary but with helper methods.
/// </summary>
public class ParsedFilter : Dictionary<string, string[]>
{
    public ParsedFilter() : base(StringComparer.OrdinalIgnoreCase) { }

    /// <summary>
    /// Returns all values for the given key, or an empty array if key not found.
    /// </summary>
    public string[] GetValues(string key)
        => TryGetValue(key, out var values) ? values : Array.Empty<string>();

    /// <summary>
    /// Checks if the filter contains a key and has at least one value.
    /// </summary>
    public bool Has(string key)
        => TryGetValue(key, out var values) && values.Length > 0;
    
    public int[] GetIntValues(string key)
    {
        if (!TryGetValue(key, out var values) || values.Length == 0)
            return Array.Empty<int>();

        return values
            .Select(v => int.TryParse(v, out var id) ? id : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id.Value)
            .ToArray();
    }

    /// <summary>
    /// Returns true if key exists and contains at least one valid integer.
    /// </summary>
    public bool HasIntValues(string key)
        => GetIntValues(key).Length > 0;
}