using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Constants;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Filtration;
using SoschedBack.Common.Http;
using SoschedBack.Storage;

namespace SoschedBack.Search.SearchUsersAndTags;

public class RequestValidator : AbstractValidator<SearchUsersAndTagsEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext dbContext, ISpaceProvider spaceProvider)
    {
    }
}