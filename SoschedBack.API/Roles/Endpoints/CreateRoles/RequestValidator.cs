using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Storage;

namespace SoschedBack.Roles.Endpoints.CreateRoles;

public class RequestValidator : AbstractValidator<CreateRolesEndpoint.Request>
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

                        var exists = await database.Roles
                            .AsNoTracking()
                            .Where(i => i.SpaceId == spaceId)
                            .AnyAsync(r => r.Name == name, cancellationToken);

                        return !exists;
                    })
                    .WithMessage("Role with this name already exists.");
            });
    }
}