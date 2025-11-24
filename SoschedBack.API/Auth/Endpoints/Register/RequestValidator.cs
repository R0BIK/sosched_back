using FluentValidation;
using SoschedBack.Auth.Endpoints.Login;
using SoschedBack.Common.Extensions;
using SoschedBack.Storage;

namespace SoschedBack.Auth.Endpoints.Register;

public class RequestValidator : AbstractValidator<RegisterEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext database)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName is required.")
            .MustBeValidName();
        
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName is required.")
            .MustBeValidName();

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Login is required.")
            .MustBeValidEmail()
            .DependentRules(() =>
            {
                RuleFor(x => x.Email)
                    .MustBeUniqueEmail(database);
            });

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MustBeValidPassword();
    }
}