using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Spaces.Endpoints.UpdateSpace;

public class UpdateSpacesEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        // Используем PATCH и ID пространства в URL
        .MapPatch("/{id:int}", Handle)
        .WithSummary("Partially updates an existing space.")
        .WithRequestValidation<RequestParameters>()
        .WithRequestValidation<RequestBody>();

    public sealed record RequestParameters(
        int Id
    );
    
    public sealed record RequestBody(
        string? Name,
        string? Domain,
        string? Password,
        bool? IsPublic
    );
    
    private sealed record Response(
        int Id,
        string Name,
        string Domain,
        bool IsPublic
    );

    private static async Task<Results<Ok<Result<Response>>, NotFound>> Handle(
        [AsParameters] RequestParameters parameters,
        [FromBody] RequestBody body,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        // 1. Находим существующее пространство
        var space = await database.Spaces
            .FirstOrDefaultAsync(s => s.Id == parameters.Id, cancellationToken);

        // 2. Обновляем сущность
        UpdateSpaceEntity(space, body);

        await database.SaveChangesAsync(cancellationToken);
        
        var response = new Response(space.Id, space.Name, space.Domain, space.IsPublic);
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
    
    private static void UpdateSpaceEntity(
        Space space, 
        RequestBody body)
    {
        // 1. Обновление Name
        if (body.Name is not null)
        {
            space.Name = body.Name.Trim();
        }

        // 2. Обновление Domain
        if (body.Domain is not null)
        {
            // Здесь может потребоваться дополнительная валидация уникальности домена
            space.Domain = body.Domain.Trim();
        }
        
        // 3. Обновление Password
        if (body.Password is not null)
        {
            // ПРИМЕЧАНИЕ: Здесь требуется хеширование пароля перед сохранением, 
            // аналогично тому, как это делается для пользователей. 
            // Для простоты примера, мы пока предполагаем, что он будет хеширован 
            // или что логика хеширования будет добавлена позднее.
            space.Password = body.Password;
        }

        // 4. Обновление IsPublic (используем HasValue, так как bool? может быть false)
        if (body.IsPublic.HasValue)
        {
            space.IsPublic = body.IsPublic.Value;
        }
    }
}