// using FluentValidation;
// using SoschedBack.Common.Extensions;
//
// namespace SoschedBack.Users.Endpoints.GetUsers;
//
// public class RequestValidator : AbstractValidator<TagTypes.Endpoints.GetTagTypes.GetTagTypesEndpoint.Request>
// {
//     public RequestValidator()
//     {
//         RuleFor(x => x.SortBy)
//             .MustBeValidSortField<TagTypes.Endpoints.GetTagTypes.GetTagTypesEndpoint.Request, AllowedSortField>();
//     }
// }