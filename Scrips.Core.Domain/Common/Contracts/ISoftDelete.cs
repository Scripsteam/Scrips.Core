namespace Scrips.Core.Domain.Common.Contracts;

public interface ISoftDelete
{
    bool IsArchived { get; set; }
}