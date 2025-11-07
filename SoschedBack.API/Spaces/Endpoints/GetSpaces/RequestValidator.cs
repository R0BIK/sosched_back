using FluentValidation;
using SoschedBack.Tags.Endpoints.GetTags;

namespace SoschedBack.Spaces.Endpoints.GetSpaces;

public class RequestValidator : AbstractValidator<GetSpacesEndpoint.Request>
{
    public RequestValidator()
    {
        
    }
}