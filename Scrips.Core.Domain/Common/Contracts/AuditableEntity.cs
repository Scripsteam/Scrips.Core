namespace Scrips.Core.Domain.Common.Contracts;

public abstract class AuditableEntity : AuditableEntity<DefaultIdType>
{
}

public abstract class AuditableEntity<T> : BaseEntity<T>, IAuditableEntity, ISoftDelete
{
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; private set; }
    public Guid UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public bool IsArchived { get; set; }
    protected AuditableEntity()
    {
        CreatedOn = DateTime.UtcNow;
        UpdatedOn = DateTime.UtcNow;
    }
}