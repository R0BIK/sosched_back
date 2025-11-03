using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Extensions;
using SoschedBack.Storage;
using SoschedBack.TagTypes.Endpoints.CreateTagTypes;

namespace SoschedBack.Auth.Endpoints.Login;

public class RequestValidator : AbstractValidator<LoginEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login is required.")
            .MustBeValidEmail();

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MustBeValidPassword();
    }
}