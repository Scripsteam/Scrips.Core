using Refit;
using Scrips.Core.Models.Practice;
using Scrips.Core.Models.Provider;

namespace Scrips.Core.HttpApiClients;

public interface IPracticeApi
{
    [Get("/api/Practice/ReminderList/{practiceId}")]
    Task<List<ReminderResponse>> PracticeReminderList(Guid practiceId, [Header("Authorization")] string token);

    [Get("/api/Doctor/AppointmentProfileList")]
    Task<AppointmentProfileResponseList> DoctorAppointmentProfileList(
        [Query] Guid practionerId,
        [Query] Guid organizationId,
        [Query] Guid practiceId,
        [Header("Authorization")] string token);

    [Get("/api/Practice/PracticeSetupList/{organizationId}")]
    Task<List<PracticeSetupList>> PracticeSetupList(Guid organizationId, [Header("Authorization")] string token);

    [Get("/api/Practice/PracticeSetupForPrimary/{organizationId}")]
    Task<PracticeSetupList> PracticeSetupForPrimary(Guid organizationId, [Header("Authorization")] string token);

    [Get("/api/Practice/PracticeSetupDetails/{practiceId}")]
    Task<PracticeDetailsResponse> PracticeSetupDetails(Guid practiceId, [Header("Authorization")] string token);

    [Post("/api/Doctor/CalendarSlots3")]
    Task<List<ProviderSearchResponse>> DoctorCalendarSlots(
        [Body] ProviderSearchRequest request,
        [Header("Authorization")] string token);

    [Post("/api/Doctor/CalendarProviders")]
    Task<List<Guid>> DoctorCalendarProviders(
        [Body] ProviderSearchRequest request,
        [Header("Authorization")] string token);

    [Get("/api/Doctor/DoctorSetupList")]
    Task<PagedResults<DoctorSetupDetails>> DoctorDoctorSetupList(
        [Header("Authorization")] string token,
        Guid organizationId,
        string q = "",
        bool? showArchived = null,
        int pageNumber = 1,
        int pageSize = 10);

    [Get("/api/Staff/StaffList?OrganizationId={organizationId}&Archive={archive}")]
    Task<List<DoctorSetupDetails>> StaffStaffList(
        Guid organizationId,
        bool archive,
        [Header("Authorization")] string token);

    [Get("/api/Doctor/GetPracticeByProviderId/{id}")]
    Task<List<PracticeSetupList>>
        PracticesByPractitionerId(Guid id, [Header("Authorization")] string authorization);

    [Get("/api/Doctor/PractitionerRoles/{organizationId}")]
    Task<PagedResults<DoctorSetupDetails>> PractitionerRolesList(
        [Header("Authorization")] string token,
        Guid organizationId,
        string types = "",
        string q = "",
        bool? showArchived = null,
        int pageNumber = 1,
        int pageSize = 10);

    [Post("/api/Doctor/CalendarSlots2")]
    Task<List<ProviderSearchResponse>> DoctorCalendarSlots2(
        [Body] ProviderSearchRequest request,
        [Header("Authorization")] string authorization);

    [Post("/api/Doctor/CalendarProviders2")]
    Task<List<Guid>> DoctorCalendarProviders2(
        [Body] ProviderSearchRequest request,
        [Header("Authorization")] string authorization);

    [Get("/api/Doctor/GetPractitionerRoleDetails/{id}")]
    Task<PractitionerResponse> GetPractitionerRoleDetails(Guid id, [Header("Authorization")] string authorization);

    [Get("/api/Practice/EditPracticeExamRoom/{practiceId}")]
    Task<List<UpdateExamRoomApiRequest>> GetPracticeRooms(
        Guid practiceId,
        [Header("Authorization")] string authorization);

    [Get("/api/Doctor/GetPracticeId/{userId}")]
    Task<DoctorData> GetPracticeId(Guid userId, [Header("Authorization")] string authorization);

    [Get("/api/Staff/GetStaffId/{userId}")]
    Task<StaffData> GetStaffId(Guid userId, [Header("Authorization")] string authorization);

    [Get("/api/Doctor/GetPractitionerRoleForUser/{userId}")]
    Task<List<PractitionerRoleDto>> GetPractitionerRoleForUser(
        Guid userId,
        Guid? organizationId,
        [Header("Authorization")] string token);
}