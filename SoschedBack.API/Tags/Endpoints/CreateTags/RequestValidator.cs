using FluentValidation;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.Regex;
using Sprache;

namespace SoschedBack.Tags.Endpoints.CreateTags;

public class RequestValidator : AbstractValidator<CreateTagsEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Name)
            .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
            .WithMessage("Title contains invalid characters.")
            .ApplyRegexPattern(RegexPatterns.Pattern.Title);
    }
}