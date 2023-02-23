using Refit;
using Scrips.Core.Models.Person;

namespace Scrips.Core.HttpApiClients;

public interface IPersonApi
{
    [Get("/api/Persons/UserDetails?userId={userId}")]
    Task<PersonInfoResponse> UserDetails([Query] Guid userId,
        [Header("Authorization")] string authorization);
}