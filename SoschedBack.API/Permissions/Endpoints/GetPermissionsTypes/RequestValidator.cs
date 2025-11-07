using FluentValidation;
using SoschedBack.EventTypes.Endpoints.GetEventTypes;

namespace SoschedBack.Permissions.Endpoints.GetPermissionsTypes;

public class RequestValidator : AbstractValidator<GetEventTypesEndpoint.Request>
{
    public RequestValidator()
    {
        
    }
}