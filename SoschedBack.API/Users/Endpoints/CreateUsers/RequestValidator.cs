using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Extensions;
using SoschedBack.Storage;

namespace SoschedBack.Users.Endpoints.CreateUsers;

public class RequestValidator : AbstractValidator<CreateUsersEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext database)
    {

    }
}