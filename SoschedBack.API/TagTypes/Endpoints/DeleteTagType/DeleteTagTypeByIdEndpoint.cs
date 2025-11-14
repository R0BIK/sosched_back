using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.TagTypes.Endpoints.DeleteTagType;

public class DeleteTagTypeByIdEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id:int}", Handle)
        .WithSummary("Deletes a tag type by id.")
        .WithRequestValidation<Request>();

    public sealed record Request(int Id);

    private static async Task<Ok<Result<string>>> Handle(
        [AsParameters] Request request,
        SoschedBackDbContext db,
        CancellationToken ct
    )
    {
        var tagType = await db.TagTypes
            .FirstAsync(t => t.Id == request.Id, ct); // гарантировано существует (валидатор проверит)

        db.TagTypes.Remove(tagType);
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(Result.Success("Tag type deleted successfully"));
    }
}