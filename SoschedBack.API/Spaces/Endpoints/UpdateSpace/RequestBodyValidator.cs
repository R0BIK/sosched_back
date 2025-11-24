using FluentValidation;
using SoschedBack.Storage;
using SoschedBack.Tags.Endpoints.UpdateTagUsers;

namespace SoschedBack.Spaces.Endpoints.UpdateSpace;

public class RequestBodyValidator : AbstractValidator<UpdateTagUsersEndpoint.RequestBody>
{
    public RequestBodyValidator(SoschedBackDbContext database)
    {
        //TODO: Validator

    }
}