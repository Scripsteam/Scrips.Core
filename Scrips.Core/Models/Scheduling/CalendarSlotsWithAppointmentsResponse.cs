using Scrips.Core.Models.Practice;
using System.Collections.Generic;

namespace Scrips.Core.Models.Scheduling;

public class CalendarSlotsWithAppointmentsResponse
{
    public List<ProviderSearchResponse> CalendarSlotsResponse { get; set; }
    public AppointmentsDetailsResponse AppointmentResponse { get; set; }
}