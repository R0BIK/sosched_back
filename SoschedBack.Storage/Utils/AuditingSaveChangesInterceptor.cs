using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Storage.Utils;

public class AuditingSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var dbContext = eventData.Context;

        if (dbContext is null)
            return base.SavingChanges(eventData, result);

        var changedEntries = dbContext.ChangeTracker
            .Entries<AuditableEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

        foreach (var entry in changedEntries)
        {
            CompleteAuditableEntity(entry.Entity, entry.State);

            // Entity should not be deleted in base.SavingChanges()
            if (entry.State == EntityState.Deleted)
                entry.State = EntityState.Modified;
        }

        return base.SavingChanges(eventData, result);
    }
    
    private void CompleteAuditableEntity(AuditableEntity entity, EntityState state)
    {
        var utcNow = DateTimeOffset.UtcNow;

        switch (state)
        {
            case EntityState.Added:
                entity.CreatedAt = utcNow;
                entity.UpdatedAt = utcNow;
                break;

            case EntityState.Modified:
                entity.UpdatedAt = utcNow;
                break;

            case EntityState.Deleted:               // Soft delete
                entity.DeletedAt = utcNow;
                entity.UpdatedAt = utcNow;
                break;
        }
    }
}