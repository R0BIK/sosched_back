// using FluentValidation;
// using Microsoft.EntityFrameworkCore;
// using SoschedBack.Common.Extensions;
// using SoschedBack.Core.Common.Regex;
// using SoschedBack.Storage;
// using SoschedBack.Tags.Endpoints.CreateTags;
//
// namespace SoschedBack.Spaces.Endpoints.CreateSpaces;
//
// public class RequestValidator : AbstractValidator<CreateSpacesEndpoint.Request>
// {
//     public RequestValidator(SoschedBackDbContext database)
//     {
//         RuleFor(x => x.Name)
//             .MustBeValidTitle();
//
//         RuleFor(x => x.Domain)
//             .MustBeValidString()
//             .WithMessage("Domain contains invalid characters.")
//             .ApplyRegexPattern(RegexPatterns.Pattern.Domain)
//             .DependentRules(() =>
//             {
//                 RuleFor(x => x.Domain)
//                     .MustAsync(async (domain, cancellationToken) =>
//                     {
//                         domain = domain.Trim();
//
//                         var exists = await database.Spaces
//                             .AsNoTracking()
//                             .AnyAsync(x => x.Name == domain, cancellationToken);
//
//                         return !exists;
//                     })
//                     .WithMessage("Domain is already taken.");
//             });
//     }
// }