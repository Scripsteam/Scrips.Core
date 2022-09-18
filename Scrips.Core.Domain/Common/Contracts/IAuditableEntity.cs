namespace Scrips.Core.Domain.Common.Contracts;

public interface IAuditableEntity
{
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; }
    public Guid UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
}