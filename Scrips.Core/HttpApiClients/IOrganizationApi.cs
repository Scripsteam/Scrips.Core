using Refit;
using Scrips.Core.Models.Organization;

namespace Scrips.Core.HttpApiClients;

public interface IOrganizationApi
{
    [Get("/api/v1/Organization/{id}")]
    Task<OrganizationDto> GetOrganization(Guid id, [Header("Authorization")] string token);

    [Get("/api/v1/OrganizationSettings/{id}")]
    Task<OrganizationSettingsDto> GetOrganizationSettings(Guid id, [Header("Authorization")] string token);

    [Post("/api/v1/Organization/list")]
    Task<List<OrganizationDto>> GetOrganizationList([Header("Authorization")] string token, [Body] object body);

    [Get("/api/Organization/UpdateAndGetOrganization/{userId}")]
    Task<OrganizationInfo> UpdateAndGetOrganization(Guid userId, [Header("Authorization")] string token);
}