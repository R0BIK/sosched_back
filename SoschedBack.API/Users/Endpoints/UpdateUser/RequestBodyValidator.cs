using FluentValidation;
using SoschedBack.Auth.Endpoints.Register;
using SoschedBack.Common.Extensions;
using SoschedBack.Storage;

namespace SoschedBack.Users.Endpoints.UpdateUser;

public class RequestValidator : AbstractValidator<UpdateUserEndpoint.RequestBody>
{
    // public RequestValidator(SoschedBackDbContext database)
    // {
    //     RuleFor(x => x.FirstName)
    //         .MustBeValidName();
    //     
    //     RuleFor(x => x.LastName)
    //         .MustBeValidName();
    //
    //     RuleFor(x => x.Email)
    //         .MustBeValidEmail()
    //         .DependentRules(() =>
    //         {
    //             RuleFor(x => x.Email)
    //                 .MustBeUniqueEmail(database);
    //         });
    //
    //     RuleFor(x => x.Password)
    //         .MustBeValidPassword();
    // }
    
    //TODO: Validator

}