using Scrips.Core.Models.Practice;

namespace Scrips.Core.Models.Scheduling;

public class CalendarSlotsWithAppointmentsResponse
{
    public List<ProviderSearchResponse> CalendarSlotsResponse { get; set; }
    public AppointmentsDetailsResponse AppointmentResponse { get; set; }
}