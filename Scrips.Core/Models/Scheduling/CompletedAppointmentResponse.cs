using Scrips.Core.Models.Practice;
using System;

namespace Scrips.Core.Models.Scheduling
{
    /// <summary>
    /// </summary>
    public class CompletedAppointmentResponse
    {
        /// <summary>
        /// </summary>
        public Guid AppointmentId { get; set; }

        /// <summary>
        /// </summary>
        public CreateAppointmentProfileRequest AppointmentProfile { get; set; }

        /// <summary>
        /// </summary>
        public DateTime AppointmentDate { get; set; }

        /// <summary>
        /// </summary>
        public ReasonCode ReasonCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AppointmentInvoiceModel Invoice { get; set; }
    }
}
