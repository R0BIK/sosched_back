using FluentValidation;
using SoschedBack.TagTypes.Endpoints.GetTagTypes;

namespace SoschedBack.EventTypes.Endpoints.GetEventTypes;

public class RequestValidator : AbstractValidator<GetEventTypesEndpoint.Request>
{
    public RequestValidator()
    {
        
    }
}