namespace Scrips.Core.Application.Auditing;

public interface IAuditService : ITransientService
{
    Task<List<AuditDto>> GetUserTrailsAsync(DefaultIdType userId);
}