using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Constants;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Auth.Authorization;

public static class ClaimsPrincipalExtensions
{
    // public static bool TryGetInstitutionId(this ClaimsPrincipal user, out int spaceId)
    // {
    //     spaceId = 0;
    //
    //     var claim = user.FindFirst(CustomClaimTypes.SpaceId);
    //     if (claim != null && int.TryParse(claim.Value, out spaceId))
    //         return true;
    //
    //     return false;
    // }
    
    public static bool TryGetUserId(this ClaimsPrincipal user, out int userId)
    {
        userId = 0;
        
        //TODO: Use Result.Success
        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null && int.TryParse(claim.Value, out userId))
            return true;

        return false;
    }

    // public static async Task<Result<int>> GetValidatedInstitutionIdAsync(
    //     this ClaimsPrincipal user,
    //     SoschedBackDbContext db,
    //     CancellationToken cancellationToken)
    // {
    //     if (!user.TryGetInstitutionId(out var institutionId))
    //     {
    //         var error = Error.From(
    //             "Institution ID is missing or invalid in the current user's claims.",
    //             "CLAIM_ID_MISSING"
    //         );
    //         return Result.Failure<int>(error);
    //     }
    //
    //     var exists = await db.Institutions
    //         .AsNoTracking()
    //         .AnyAsync(i => i.Id == institutionId, cancellationToken);
    //
    //     if (!exists)
    //     {
    //         var error = Error.From(
    //             $"Institution with ID '{institutionId}' does not exist.",
    //             "ENTITY_DOES_NOT_EXIST"
    //         );
    //         return Result.Failure<int>(error);
    //     }
    //
    //     return Result.Success(institutionId);
    // }
    //
    // public static async Task<Result<int>> GetValidatedUserIdAsync(
    //     this ClaimsPrincipal user,
    //     SoschedBackDbContext db,
    //     CancellationToken cancellationToken)
    // {
    //     if (!user.TryGetUserId(out var userId))
    //     {
    //         var error = Error.From(
    //             "User ID is missing or invalid in the current user's claims.",
    //             "CLAIM_ID_MISSING"
    //         );
    //         return Result.Failure<int>(error);
    //     }
    //
    //     var exists = await db.Users
    //         .AsNoTracking()
    //         .AnyAsync(i => i.Id == userId, cancellationToken);
    //
    //     if (!exists)
    //     {
    //         var error = Error.From(
    //             $"User with ID '{userId}' does not exist.",
    //             "ENTITY_DOES_NOT_EXIST"
    //         );
    //         return Result.Failure<int>(error);
    //     }
    //
    //     return Result.Success(userId);
    // }
}