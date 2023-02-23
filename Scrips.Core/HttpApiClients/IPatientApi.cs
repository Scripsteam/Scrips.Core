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

    [Get("/api/Patients/{patientId}/HealthInsurance/{companyCode}")]
    Task<ApiResponse<HealthInsuranceResponse>> PatientHealthInsuranceByPatientIdForCompanyCode(
        Guid patientId,
        string? companyCode,
        [Header("Authorization")] string authorization);

    [Get("/api/Patients/GetGuardianDetails?userId={userId}")]
    Task<GuardianDto> GetGuardianDetails(Guid userId, [Header("Authorization")] string authorization);
}