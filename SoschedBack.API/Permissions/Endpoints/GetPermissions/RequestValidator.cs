using FluentValidation;

namespace SoschedBack.Permissions.Endpoints.GetPermissions;

public class RequestValidator : AbstractValidator<GetPermissionsEndpoint.Request>
{
    public RequestValidator()
    {
        
    }
}