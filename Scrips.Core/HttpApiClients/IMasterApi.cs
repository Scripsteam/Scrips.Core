using Refit;
using Scrips.Core.Models;

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
}