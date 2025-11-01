using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Extensions;
using SoschedBack.Storage;
using SoschedBack.Tags.Endpoints.CreateTags;

namespace SoschedBack.TagTypes.Endpoints.CreateTagTypes;

public class RequestValidator : AbstractValidator<CreateTagTypesEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext database)
    {
        RuleFor(x => x.Name)
            .MustBeValidTitle()
            .DependentRules(() =>
            {
                RuleFor(x => x.Name)
                    .MustAsync(async (name, cancellationToken) =>
                    {
                        name = name.Trim();

                        var exists = await database.Tags
                            .AsNoTracking()
                            .AnyAsync(x => x.Name == name, cancellationToken);

                        return !exists;
                    })
                    .WithMessage("Name already exists.");
            });

    }
}