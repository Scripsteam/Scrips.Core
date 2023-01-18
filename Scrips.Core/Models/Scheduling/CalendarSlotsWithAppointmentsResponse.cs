using Scrips.Core.Models.Practice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrips.Core.Models.Scheduling
{
    public class CalendarSlotsWithAppointmentsResponse
    {
        public List<ProviderSearchResponse> CalendarSlotsResponse { get; set; }
        public AppointmentsDetailsResponse AppointmentResponse { get; set; }
    }
}
