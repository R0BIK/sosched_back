// using FluentValidation;
// using SoschedBack.Common.Extensions;
//
// namespace SoschedBack.Users.Endpoints.GetUserById;
//
// public class RequestValidator : AbstractValidator<GetUserByIdEndpoint.Request>
// {
//     public RequestValidator()
//     {
//         RuleFor(x => x.Id)
//             .MustBeValidId();
//     }
// }