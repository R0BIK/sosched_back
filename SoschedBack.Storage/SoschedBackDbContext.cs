using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Core.Models;
using SoschedBack.Core.Models.Interfaces;
using SoschedBack.Storage.Utils;

namespace SoschedBack.Storage;

public partial class SoschedBackDbContext : DbContext
{
    public SoschedBackDbContext()
    {
    }

    public SoschedBackDbContext(DbContextOptions<SoschedBackDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .AddInterceptors(new AuditingSaveChangesInterceptor())
            .UseLazyLoadingProxies()
            .EnableSensitiveDataLogging();
    
    public DbSet<User> Users { get; set; } = null!;
    
    public DbSet<Event> Events { get; set; } = null!;
    
    public DbSet<Permission> Permissions { get; set; } = null!;
    
    public DbSet<Role> Roles { get; set; } = null!;
    
    public DbSet<Tag> Tags { get; set; } = null!;
    
    public DbSet<TagToUser> TagToUsers { get; set; } = null!;
    
    public DbSet<EventToSpaceUser> EventToSpaceUsers { get; set; } = null!;
    
    public DbSet<PermissionToRole> PermissionToRoles { get; set; } = null!;
    
    public DbSet<EventType> EventTypes { get; set; } = null!;
    
    public DbSet<Space> Spaces { get; set; } = null!;
    
    public DbSet<SpaceUser> SpaceUsers { get; set; } = null!;
    
    public DbSet<TagType> TagTypes { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = GetType().Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);

        AddConfigurationOnAuditableEntity(modelBuilder);

        OnModelCreatingPartial(modelBuilder);
    }
    
    private void AddConfigurationOnAuditableEntity(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.ClrType.IsAssignableTo(typeof(AuditableEntity)) &&
                entityType.BaseType == null)
            {
                var method = typeof(SoschedBackDbContext)
                    .GetMethod(nameof(ConfigureAuditableEntity), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, [ modelBuilder ]);
            }
        }
    }

    private static void ConfigureAuditableEntity<T>(ModelBuilder modelBuilder) where T : AuditableEntity
    {
        ConfigureFieldsForAuditableEntity<T>(modelBuilder);
        SetQueryFilterOnSoftDelete<T>(modelBuilder);
    }

    private static void ConfigureFieldsForAuditableEntity<T>(ModelBuilder modelBuilder) where T : AuditableEntity
    {
        modelBuilder.Entity<T>()
            .Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired(true);

        modelBuilder.Entity<T>()
            .Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired(true);

        modelBuilder.Entity<T>()
            .Property(e => e.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
    }

    private static void SetQueryFilterOnSoftDelete<T>(ModelBuilder modelBuilder) where T : AuditableEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => e.DeletedAt == null);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}