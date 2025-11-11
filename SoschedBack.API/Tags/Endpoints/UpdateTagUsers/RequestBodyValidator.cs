using FluentValidation;
using SoschedBack.Spaces.Endpoints.UpdateSpaceUsers;
using SoschedBack.Storage;

namespace SoschedBack.Tags.Endpoints.UpdateTagUsers;

public class RequestBodyValidator : AbstractValidator<UpdateTagUsersEndpoint.RequestBody>
{
    public RequestBodyValidator(SoschedBackDbContext database)
    {
        
    }
}