// using Microsoft.EntityFrameworkCore;
// using SoschedBack.Common;
// using SoschedBack.Common.Extensions;
// using SoschedBack.Core.Common.UnifiedResponse;
// using SoschedBack.Storage;
//
// namespace SoschedBack.Users.Endpoints.GetUserById;
//
// public class GetUserByIdEndpoint : IEndpoint
// {
//     public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
//         .MapGet("/{id}", Handle)
//         .WithSummary("Returns a user by id.")
//         .WithRequestValidation<Request>();
//
//     public sealed record Request(
//         int Id
//     );
//
//     public sealed record Response(
//         int Id,
//         int RoleId,
//         string FirstName,
//         string LastName,
//         string Patronymic,
//         string Email,
//         string Password,
//         DateOnly BirthDate,
//         string IconPath
//     );
//
//     private static async Task<IResult> Handle(
//         [AsParameters] Request request,
//         SoschedBackDbContext database,
//         CancellationToken ct
//     )
//     {
//         var tagType = await database.Tags
//             .FirstOrDefaultAsync(i => i.Id == request.Id, ct);
//
//         if (tagType == null)
//         {
//             var error = Error.From(
//                 $"Tag type with id {request.Id} does not exist.",
//                 "ENTITY_DOES_NOT_EXISTS"
//             );
//             
//             return Results.NotFound(error);
//         }
//
//         var response = new Response(
//             tagType.Id,
//             tagType.Name
//         );
//         
//         var result = Result.Success(response);
//         
//         return Results.Ok(result);
//     }
// }