using Refit;
using Scrips.Core.Models.Provider;

namespace Scrips.Core.HttpApiClients;

public interface IProviderApi
{
    [Get("/api/ProfessionalDetails/GetProviderReasonForVisits/{providerId}")]
    Task<List<ProviderReasonCodeModel>> ProfessionalDetailsGetProviderReasonForVisits(Guid providerId,
        [Header("Authorization")] string token);

    [Get("/api/ProfessionalDetails/{providerId}")]
    Task<ProfessionalDetailRequestModel> ProfessionalDetailsGetProfessionalDetail(Guid providerId,
        [Header("Authorization")] string token);

    [Get("/api/Provider/{id}")]
    Task<PractitionerResponse> ProviderGet(Guid id, [Header("Authorization")] string token);

    [Get("/api/Provider/Id/{id}")]
    Task<PractitionerResponse> PractitionerGet(Guid id, [Header("Authorization")] string token);

    [Get("/api/Provider")]
    Task<List<ProviderListModel>> ProviderGetAll([Query] ProviderRequestModel request, [Header("Authorization")] string token);

}