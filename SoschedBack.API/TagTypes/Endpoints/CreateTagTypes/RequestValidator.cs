using System.Linq.Dynamic.Core;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Storage;
using SoschedBack.Tags.Endpoints.CreateTags;

namespace SoschedBack.TagTypes.Endpoints.CreateTagTypes;

public class RequestValidator : AbstractValidator<CreateTagTypesEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext database, ISpaceProvider spaceProvider)
    {
        var spaceId = spaceProvider.GetSpace();
        
        RuleFor(x => x.Name)
            .MustBeValidTitle()
            .DependentRules(() =>
            {
                RuleFor(x => x.Name)
                    .MustAsync(async (name, cancellationToken) =>
                    {
                        name = name.Trim();

                        var exists = await database.TagTypes
                            .AsNoTracking()
                            .Where(tt => tt.SpaceId == spaceId)
                            .AnyAsync(x => x.Name == name, cancellationToken);

                        return !exists;
                    })
                    .WithMessage("Name already exists.");
            });

    }
}