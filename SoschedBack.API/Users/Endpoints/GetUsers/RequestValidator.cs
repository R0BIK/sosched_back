using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Constants;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Filtration;
using SoschedBack.Common.Http;
using SoschedBack.Storage;

namespace SoschedBack.Users.Endpoints.GetUsers;

public class RequestValidator : AbstractValidator<GetUsersEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext dbContext, ISpaceProvider spaceProvider)
    {
        RuleFor(x => x.Filter)
            .MustBeValidOptionalString()
            .DependentRules(() =>
            {
                RuleFor(x => x.Filter)
                    .CustomAsync(async (filter, context, ct) =>
                    {
                        if (string.IsNullOrWhiteSpace(filter))
                            return;

                        var spaceId = spaceProvider.GetSpace();
                
                        var parsedFilters = FilterParser.Parse(filter);
                        if (parsedFilters.Count == 0)
                        {
                            context.AddFailure("Invalid filter format.");
                            return;
                        }
                        
                        foreach (var (key, values) in parsedFilters)
                        {
                            if (key.Equals(FilterConstants.RoleKey, StringComparison.OrdinalIgnoreCase))
                            {
                                await ValidateRolesAsync(values, spaceId, dbContext, context, ct);
                            }
                            else if (key.StartsWith(FilterConstants.TagKey, StringComparison.OrdinalIgnoreCase))
                            {
                                await ValidateTagsAsync(values, spaceId, dbContext, context, ct);
                            }
                            else if (key.StartsWith(FilterConstants.EventKey, StringComparison.OrdinalIgnoreCase))
                            {
                                await ValidateEventsAsync(parsedFilters.GetIntValues(FilterConstants.EventKey), spaceId, dbContext, context, ct);
                            }
                            else
                            {
                                context.AddFailure($"Unknown filter key for this entity: '{key}'.");
                            }
                        }
                    });
            });
    }
    private static async Task ValidateEventsAsync(
        IEnumerable<int> values,
        int spaceId,
        SoschedBackDbContext dbContext,
        CustomContext context,
        CancellationToken ct)
    {
        var existingEvents = await dbContext.Events
            .AsNoTracking()
            .Where(t => t.SpaceId == spaceId && values.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync(ct);

        var invalidEvents = values
            .Except(existingEvents)
            .ToArray();

        if (invalidEvents.Length > 0)
        {
            context.AddFailure($"Invalid events: {string.Join(", ", invalidEvents)}.");
        }
    }
    
    private static async Task ValidateRolesAsync(
        IEnumerable<string> values,
        int spaceId,
        SoschedBackDbContext dbContext,
        CustomContext context,
        CancellationToken ct)
    {
        var existingRoles = await dbContext.Roles
            .AsNoTracking()
            .Where(r => r.SpaceId == spaceId && values.Contains(r.Name))
            .Select(r => r.Name)
            .ToListAsync(ct);

        var invalidRoles = values.Except(existingRoles, StringComparer.OrdinalIgnoreCase).ToArray();
        if (invalidRoles.Length > 0)
            context.AddFailure($"Invalid roles: {string.Join(", ", invalidRoles)}.");
    }
    
    private static async Task ValidateTagsAsync(
        IEnumerable<string> values,
        int spaceId,
        SoschedBackDbContext dbContext,
        CustomContext context,
        CancellationToken ct)
    {
        var existingTags = await dbContext.Tags
            .AsNoTracking()
            .Where(t => t.SpaceId == spaceId && values.Contains(t.ShortName))
            .Select(t => t.ShortName)
            .ToListAsync(ct);

        var invalidTags = values
            .Except(existingTags, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (invalidTags.Length > 0)
        {
            context.AddFailure($"Invalid tags: {string.Join(", ", invalidTags)}.");
        }
    }
}