using FluentValidation;
using SoschedBack.Storage;
using SoschedBack.Tags.Endpoints.UpdateTagUsers;

namespace SoschedBack.Events.Endpoints.UpdateEventUsers;

public class RequestBodyValidator : AbstractValidator<UpdateEventUsersEndpoint.RequestBody>
{
    public RequestBodyValidator(SoschedBackDbContext database)
    {
        
    }
}