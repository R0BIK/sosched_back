using System.Linq.Dynamic.Core;
using System.Reflection;
using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Common.Extensions;

public static class QueryExtensions
{
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        string? sortBy,
        bool descending,
        string? defaultSortField = nameof(AuditableEntity.CreatedAt))
        where T : AuditableEntity
    {
        if (string.IsNullOrEmpty(sortBy) || !IsValidProperty<T>(sortBy))
        {
            sortBy = defaultSortField;
        }

        var direction = descending ? "desc" : "asc";
        return query.OrderBy($"{sortBy} {direction}");
    }

    private static bool IsValidProperty<T>(string propertyName)
    {
        return typeof(T).GetProperty(propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) != null;
    }
}