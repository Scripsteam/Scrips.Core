using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using Scrips.Core.Models.Identity;

namespace Scrips.Core.HttpApiClients;

public interface IIdentityApi
{
    [Post("/api/v1/Users/ContactDetails")]
    Task<List<ContactDto>> GetContactDetails([Body] GetUserDetailsRequest request, [Header("Authorization")] string token);
        
    [Get("/api/Tenants/{tenantId}")]
    Task<TenantDto> GetTenant(string? tenantId, [Header("Authorization")] string token);
}