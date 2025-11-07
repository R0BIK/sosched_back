using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Constants;
using SoschedBack.Storage;

namespace SoschedBack.Common.Filtration;

/// <summary>
/// Parses filter query string into structured key-value representation.
/// Example: "role=admin,student;tagtype_group=ip-33,ik-31;tagtype_main=teacher"
/// </summary>
public static class FilterParser
{
    /// <summary>
    /// Parses a filter string (e.g., "role=admin,student;tagtype_main=teacher")
    /// into a <see cref="ParsedFilter"/> object.
    /// </summary>
    public static ParsedFilter Parse(string? filter)
    {
        var result = new ParsedFilter();

        if (string.IsNullOrWhiteSpace(filter))
            return result;

        var parts = filter.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            var kv = part.Split('=', 2);
            if (kv.Length != 2)
                continue;

            var key = kv[0].Trim();
            var values = kv[1]
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim())
                .ToArray();

            result.Add(key, values);
        }

        return result;
    }
}