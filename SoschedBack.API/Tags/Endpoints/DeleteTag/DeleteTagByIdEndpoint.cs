using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Tags.Endpoints.DeleteTag;

public class DeleteTagByIdEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id:int}", Handle)
        .WithSummary("Deletes a tag by id. Removes it from all users who have it.")
        .WithRequestValidation<Request>();

    public sealed record Request(int Id);

    private static async Task<Ok<Result<string>>> Handle(
        [AsParameters] Request request,
        SoschedBackDbContext db,
        CancellationToken ct
    )
    {
        // Получаем тег (гарантировано существует, валидатор)
        var tag = await db.Tags
            .FirstAsync(t => t.Id == request.Id, ct);

        // Удаляем все связи Tag -> SpaceUser
        var tagRelations = await db.TagToSpaceUsers
            .Where(tsu => tsu.TagId == tag.Id)
            .ToListAsync(ct);

        if (tagRelations.Any())
        {
            db.TagToSpaceUsers.RemoveRange(tagRelations);
        }

        // Удаляем сам тег
        db.Tags.Remove(tag);

        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(Result.Success("Tag deleted successfully"));
    }
}