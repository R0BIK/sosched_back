using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Users.Endpoints.GetUserById;

public class RequestValidator : AbstractValidator<GetUserByIdEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext db, ISpaceProvider spaceProvider)
    {
        var spaceId = spaceProvider.GetSpace();
        
        RuleFor(x => x.Id)
            .MustBeValidId()
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .MustBeValidEntityId<GetUserByIdEndpoint.Request, User>(db)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.Id)
                            .MustAsync(async (id, cancellationToken) =>
                            {
                                return await db.SpaceUsers
                                    .AnyAsync(x => x.UserId == id && x.SpaceId == spaceId, cancellationToken);
                            })
                            .WithMessage((_, id) => $"User with id {id} not found in current space.");
                    });
            });
    }
}