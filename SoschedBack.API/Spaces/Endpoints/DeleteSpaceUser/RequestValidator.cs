// using FluentValidation;
// using SoschedBack.Common.Extensions;
// using SoschedBack.Core.Common.Regex;
// using SoschedBack.Core.Models;
// using SoschedBack.Spaces.Endpoints.GetSpaceById;
// using SoschedBack.Storage;
//
// namespace SoschedBack.Spaces.Endpoints.GetSpaceByDomain;
//
// public class RequestValidator : AbstractValidator<GetSpaceByDomainEndpoint.Request>
// {
//     public RequestValidator(SoschedBackDbContext db)
//     {
//         RuleFor(x => x.Domain)
//             .MustBeValidString()
//             .WithMessage("Domain contains invalid characters.")
//             .ApplyRegexPattern(RegexPatterns.Pattern.Domain)
//             .DependentRules(() =>
//             {
//                 RuleFor(x => x.Domain)
//                     .MustBeValidEntityId<GetSpaceByIdEndpoint.Request, Space>(db);
//             });
//     }
// }

//TODO: validation