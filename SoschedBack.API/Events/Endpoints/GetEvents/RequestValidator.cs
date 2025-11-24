using FluentValidation;
using SoschedBack.Common.Extensions;
using SoschedBack.Tags.Endpoints.GetTags;

namespace SoschedBack.Events.Endpoints.GetEvents;

public class RequestValidator : AbstractValidator<GetTagsEndpoint.Request>
{
    public RequestValidator()
    {
        //TODO: Validator
    }
}