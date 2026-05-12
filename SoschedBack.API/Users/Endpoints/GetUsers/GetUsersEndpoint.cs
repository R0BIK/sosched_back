using System.Linq.Dynamic.Core;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Constants;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Filtration;
using SoschedBack.Common.Http;
using SoschedBack.Common.Pagination;
using SoschedBack.Common.Pagination.PagedRequest;
using SoschedBack.Common.Requests;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Users.Endpoints.GetUsers;

public class GetUsersEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of users.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int? Page = 1,
        int? PageSize = 10,
        string? SortBy = null,
        bool Descending = false,
        string? Filter = null,
        string? Search = null
    ) : IPagedRequest, ISortRequest;

    private sealed record UserTags(
        int Id,
        string ShortName,
        string Color
    );
    
    private sealed record Response(
        int Id,
        string FirstName,
        string LastName,
        string? Patronymic,
        string Email,
        string IconPath,
        string Role,
        List<UserTags> UserTags
    );

    private static async Task<Ok<Result<PagedList<Response>>>> Handle(
        [AsParameters] Request request,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        // var requestJson = System.Text.Json.JsonSerializer.Serialize(request);
        // Console.WriteLine("Request object: " + requestJson);
        
        var spaceId = spaceProvider.GetSpace();
        
        var filters = FilterParser.Parse(request.Filter);

        var query = ApplyFilters(spaceId, filters, request.Search, database);
        
        //TODO: Apply sorting
        
        var users = await query
            .AsNoTracking()
            .Select(user => new Response(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Patronymic,
                user.Email,
                user.IconPath,
                user.SpaceUsers
                    .Where(su => su.SpaceId == spaceId)
                    .Select(su => su.Role.Name)
                    .First(),
                user.SpaceUsers
                    .Where(su => su.SpaceId == spaceId)
                    .SelectMany(su => su.TagToSpaceUsers)
                    .Select(tsu => new UserTags(
                        tsu.Tag.Id,
                        tsu.Tag.ShortName,
                        tsu.Tag.Color
                    ))
                    .ToList()
            ))
            .ToPagedListAsync(request, ct);
        
        var result = Result.Success(users);
        
        return TypedResults.Ok(result);
    }

    private static IQueryable<User> ApplyFilters(
        int spaceId,
        ParsedFilter filters,
        string? search,
        SoschedBackDbContext dbContext
    )
    {
        var baseQuery = dbContext.Users
            .AsNoTracking()
            .Where(u => u.SpaceUsers.Any(su => su.SpaceId == spaceId));
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = $"%{search.ToLower()}%";

            baseQuery = baseQuery.Where(u =>
                EF.Functions.ILike(u.FirstName + " " + u.LastName + " " + (u.Patronymic ?? ""), searchLower) ||
                EF.Functions.ILike(u.FirstName + " " + (u.Patronymic ?? "") + " " + u.LastName, searchLower) ||
                EF.Functions.ILike(u.LastName + " " + u.FirstName + " " + (u.Patronymic ?? ""), searchLower) ||
                EF.Functions.ILike(u.LastName + " " + (u.Patronymic ?? "") + " " + u.FirstName, searchLower) ||
                EF.Functions.ILike((u.Patronymic ?? "") + " " + u.FirstName + " " + u.LastName, searchLower) ||
                EF.Functions.ILike((u.Patronymic ?? "") + " " + u.LastName + " " + u.FirstName, searchLower) ||
                EF.Functions.ILike(u.FirstName, searchLower) ||
                EF.Functions.ILike(u.LastName, searchLower) ||
                EF.Functions.ILike(u.Patronymic ?? "", searchLower) ||
                EF.Functions.ILike(u.Email, searchLower)
            );
        }
        
        if (filters.Has(FilterConstants.RoleKey))
        {
            var roles = filters.GetValues(FilterConstants.RoleKey);
            baseQuery = baseQuery
                .Where(u => u.SpaceUsers.Any(su => roles.Contains(su.Role.Name) && su.SpaceId == spaceId));
        }
        
        // foreach (var key in filters.Keys.Where(k => k.StartsWith(FilterConstants.TagTypePrefix, StringComparison.OrdinalIgnoreCase)))
        // {
        //     var tagTypeName = key[FilterConstants.TagTypePrefix.Length..];
        //     var tagNames = filters.GetValues(key);
        //
        //     baseQuery = baseQuery.Where(u =>
        //         u.SpaceUsers.Any(su =>
        //             su.TagToSpaceUsers.Any(tsu =>
        //                 tsu.Tag.TagType.Name == tagTypeName &&
        //                 tagNames.Contains(tsu.Tag.Name) &&
        //                 tsu.SpaceUser.SpaceId == spaceId
        //             )
        //         )
        //     );
        // }
        
        if (filters.Has(FilterConstants.TagKey))
        {
            baseQuery = ApplySmartTagFilter(baseQuery, spaceId, filters, dbContext);
            // var tags = filters.GetValues(FilterConstants.TagKey);
            // baseQuery = baseQuery
            //     .Where(u => u.SpaceUsers.Any(su => su.TagToSpaceUsers.Any(sut => tags.Contains(sut.Tag.ShortName) && sut.SpaceUser.SpaceId == spaceId)));
        }
        
        if (filters.Has(FilterConstants.EventKey))
        {
            var events = filters.GetIntValues(FilterConstants.EventKey);
            baseQuery = baseQuery
                .Where(u => u.SpaceUsers.Any(su => su.EventToSpaceUsers.Any(sut => events.Contains(sut.Event.Id) && sut.SpaceUser.SpaceId == spaceId)));
        }

        return baseQuery;
    }
    
    private static IQueryable<User> ApplySmartTagFilter(
        IQueryable<User> query,
        int spaceId,
        ParsedFilter filters,
        SoschedBackDbContext dbContext)
    {
        // Проверяем, есть ли фильтр 'tags' (или как он у тебя в константах)
        if (!filters.Has(FilterConstants.TagKey))
        {
            return query;
        }

        // 1. Получаем список имен тегов с фронта (например: ["ИП-31", "ИП-32", "Староста"])
        var requestedTagNames = filters.GetValues(FilterConstants.TagKey);

        if (requestedTagNames == null || !requestedTagNames.Any())
        {
            return query;
        }

        // 2. Узнаем TagTypeId для этих тегов (легкий запрос)
        // Группируем теги, чтобы понять: какие из них "однотипные" (ИЛИ), а какие "разные" (И)
        var tagsInfo = dbContext.Tags
            .AsNoTracking()
            .Where(t => t.SpaceId == spaceId && requestedTagNames.Contains(t.ShortName))
            .Select(t => new { t.ShortName, t.TagTypeId })
            .ToList();

        var tagsByGroup = tagsInfo.GroupBy(t => t.TagTypeId);

        // 3. Динамически накладываем фильтры
        foreach (var group in tagsByGroup)
        {
            var namesInThisType = group.Select(x => x.ShortName).ToArray();

            // Добавляем условие Where (AND). Внутри условия используем Contains (OR).
            // Логика: Пользователь должен иметь ( (ТегА ИЛИ ТегБ из Группы1) И (ТегС из Группы2) )
            query = query.Where(u =>
                u.SpaceUsers.Any(su =>
                    su.SpaceId == spaceId &&
                    su.TagToSpaceUsers.Any(tsu =>
                            tsu.Tag.TagTypeId == group.Key &&     // Тег относится к текущей группе
                            namesInThisType.Contains(tsu.Tag.ShortName) // Имя тега совпадает с одним из запрошенных
                    )
                )
            );
        }

        return query;
    }
}