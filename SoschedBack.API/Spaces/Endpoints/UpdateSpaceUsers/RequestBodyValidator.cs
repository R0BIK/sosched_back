using FluentValidation;
using SoschedBack.Storage;

namespace SoschedBack.Spaces.Endpoints.UpdateSpaceUsers;

public class RequestBodyValidator : AbstractValidator<UpdateSpaceUsersEndpoint.RequestBody>
{
    public RequestBodyValidator(SoschedBackDbContext database)
    {
        //TODO: Validator

    }
}