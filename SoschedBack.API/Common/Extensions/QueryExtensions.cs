using System.Linq.Dynamic.Core;
using System.Reflection;
using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Common.Extensions;

public static class QueryExtensions
{
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        string? sortBy,
        bool descending)
    {
        var defaultSortField = typeof(AuditableEntity).IsAssignableFrom(typeof(T))
            ? nameof(AuditableEntity.CreatedAt)
            : null;
        
        if (string.IsNullOrWhiteSpace(sortBy) || !IsValidProperty<T>(sortBy))
            sortBy = defaultSortField;

        if (string.IsNullOrWhiteSpace(sortBy))
            return query;

        var direction = descending ? "desc" : "asc";
        return query.OrderBy($"{sortBy} {direction}");
    }
    
    private static bool IsValidProperty<T>(string propertyName)
    {
        return typeof(T).GetProperty(propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) != null;
    }
}