// using FluentValidation;
// using SoschedBack.Common.Http;
// using SoschedBack.Storage;
//
// namespace SoschedBack.Tags.Endpoints.UpdateTagUsers;
//
// public class RequestParametersValidator : AbstractValidator<UpdateTagUsersEndpoint.RequestParameters>
// {
//     public RequestParametersValidator(SoschedBackDbContext db, ISpaceProvider spaceProvider)
//     {
//         var institutionId = institutionProvider.GetInstitutionId();
//
//         RuleFor(x => x.Id)
//             .MustBeValidId()
//             .DependentRules(() =>
//             {
//                 RuleFor(x => x.Id)
//                     .MustBeValidInstitutionEntityId<UpdateGroupStudentsEndpoint.RequestParameters, Group>(db, institutionId);
//             });
//     }
// }