using System.ComponentModel.DataAnnotations;

namespace AspDotnetBoilerplate.src.Shared.Utils;

public abstract class AuditableEntity<TKey>
{
    // UUID por padrão
    public TKey Id { get; } = default!;

    // dados de auditoria
    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    [Timestamp]
    public uint RowVersion { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public Guid? DeletedBy { get; set; }

    public void SoftDelete(Guid? deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = default;
        DeletedBy = default;
    }
}