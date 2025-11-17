// using FluentValidation;
// using SoschedBack.Common.Extensions;
// using SoschedBack.Common.Http;
// using SoschedBack.Core.Models;
// using SoschedBack.Storage;
//
// namespace SoschedBack.Permissions.Endpoints.GetPermissionById;
//
// public class RequestValidator : AbstractValidator<GetPermissionByIdEndpoint.Request>
// {
//     public RequestValidator(SoschedBackDbContext db, ISpaceProvider spaceProvider)
//     {
//         RuleFor(x => x.Id)
//             .MustBeValidId()
//             .DependentRules(() =>
//             {
//                 RuleFor(x => x.Id)
//                     .MustBeValidEntityId<GetPermissionByIdEndpoint.Request, EventType>(db);
//             });
//     }
// }