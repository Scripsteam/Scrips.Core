using Refit;
using Scrips.Core.Models;
using Scrips.Core.Models.AIChiefComplaint;

namespace Scrips.Core.HttpApiClients;

public interface IMasterApi
{
    [Get("/api/Master/Gender")]
    Task<List<IdNamePair>> MasterGetGender([Header("Authorization")] string token);
    [Get("/api/Master/IdentityType")]
    Task<List<IdNamePair>>  MasterGetIdentityType([Header("Authorization")] string token);
    [Get("/api/Master/MaritalStatus")]
    Task<List<IdNamePair>>  MasterGetMartialStatus([Header("Authorization")] string token);
    [Get("/api/Master/OwnerType")]
    Task<List<IdNamePair>>  MasterGetOwnerType([Header("Authorization")] string token);
    [Post("/api/AISearch/RAGSearch")]
    Task<RAGSearchResult> RAGSearch(RAGSearchRequest request, [Header("Authorization")] string token);
    [Post("/api/AISearch/ClinicalSuggestions")]
    Task<IReadOnlyList<RAGSuggestion>> ClinicalSuggestions([Body] ClinicalSuggestionsRequest request, [Header("Authorization")] string token);
    [Post("/api/AISearch/GenerateDocumentation")]
    Task<DocumentationResponse> GenerateDocumentation([Body] DocumentationRequest request, [Header("Authorization")] string token);
    [Get("/api/AISearch/ByNameAndCategory")]
    Task<ChiefComplaintDto> ByNameAndCategory( string chiefComplaint, string category, [Header("Authorization")] string token);
}