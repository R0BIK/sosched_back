using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Tags.Endpoints.CreateTags;

public class RequestValidator : AbstractValidator<CreateTagsEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext database, ISpaceProvider spaceProvider)
    {
        var spaceId = spaceProvider.GetSpace();
        
        RuleFor(x => x.Name)
            .MustBeValidTitle();
        
        RuleFor(x => x.ShortName)
            .MustBeValidTitle();

        RuleFor(x => x.TagType)
            .MustBeValidId()
            .DependentRules(() =>
            {
                RuleFor(x => x.TagType)
                    .MustBeValidSpaceEntityId<CreateTagsEndpoint.Request, TagType>(database, spaceId);
            });

        RuleFor(x => x)
            .CustomAsync(async (request, context, cancellationToken) =>
            {
                if (!IsSuccessfullValidation(request)) return;
                
                var name = request.Name.Trim();
                var shortName = request.ShortName.Trim();
                var tagType = request.TagType;

                var existing = await database.Tags
                    .AsNoTracking()
                    .Where(t => t.SpaceId == spaceId)
                    .Where(i =>
                        i.Name == name ||
                        i.ShortName == shortName ||
                        i.TagType.Id == tagType
                    )
                    .Select(i => new { i.Name, i.ShortName, i.TagType.Id })
                    .FirstOrDefaultAsync(cancellationToken);
                
                if (existing is not null)
                {
                    if (existing.Name == name)
                        context.AddFailure($"Tag with name {name} already exists.");

                    if (existing.ShortName == shortName)
                        context.AddFailure($"Tag with short name {shortName} already exists.");

                    if (existing.Id != tagType)
                        context.AddFailure($"Tag type with id {tagType} does not exist.");
                }
            });
    }

    private static bool IsSuccessfullValidation(CreateTagsEndpoint.Request request)
    {
        var validator = new InlineValidator<CreateTagsEndpoint.Request>();

        validator.RuleFor(x => x.Name).MustBeValidTitle();
        validator.RuleFor(x => x.ShortName).MustBeValidTitle();
        validator.RuleFor(x => x.TagType).MustBeValidId();

        var result = validator.Validate(request);

        return result.IsValid;
    }
}