using Refit;

namespace Scrips.Core.HttpApiClients;

public interface INotificationsApi
{
    [Get("/api/Notifications/GetVideoUrl")]
    Task<string> GetVideoUrl(Guid appointmentId, Guid patientId, string notificationType);
}