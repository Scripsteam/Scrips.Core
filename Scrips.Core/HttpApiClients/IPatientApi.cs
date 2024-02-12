using Refit;
using Scrips.Core.Models.Patient;

namespace Scrips.Core.HttpApiClients;

public interface IPatientApi
{
    [Get("/api/Patients/{id}")]
    Task<EditPatientResponse> Get(Guid id, [Header("Authorization")] string auth);

    [Get("/api/Patients/PatientAddressList")]
    Task<List<PatientAddressListResponse>> PatientAddressList(
        [Query] Guid patientId,
        [Header("Authorization")] string authorization);

    [Get("/api/Patients/{patientId}/HealthInsuranceSponsor/{sponsorId}")]
    Task<ApiResponse<HealthInsuranceResponse>> HealthInsuranceByPatientIdForSponsorId(
        Guid patientId,
        Guid sponsorId,
        [Header("Authorization")] string authorization);

    [Get("/api/Patients/{patientId}/PatientCorporateSponsor/{sponsorId}")]
    Task<ApiResponse<PatientCorporateResponse>> PatientCorporateByPatientIdForSponsorId(
    Guid patientId,
    Guid sponsorId,
    [Header("Authorization")] string authorization);

    [Get("/api/Patients/GetGuardianDetails?userId={userId}")]
    Task<GuardianDto> GetGuardianDetails(Guid userId, [Header("Authorization")] string authorization);
}