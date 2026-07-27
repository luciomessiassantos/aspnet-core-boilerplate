using AspDotnetBoilerplate.src.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace AspDotnetBoilerplate.src.Infrastructure;


public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }

    private static void ConfigureAuditableEntity<T, TKey>(ModelBuilder modelBuilder, string tableName)
        where T : AuditableEntity<TKey>
    {
        modelBuilder.Entity<T>(entity =>
        {
            entity.ToTable(tableName);

            entity.HasKey(e => e.Id);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.DeletedAt);
            entity.Property(e => e.DeletedBy);


            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }

 public override int SaveChanges()
    {
        OnSaving();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnSaving();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void OnSaving()
    {
        var entries = ChangeTracker.Entries<AuditableEntity<Guid>>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
            }

        }
    }   
}