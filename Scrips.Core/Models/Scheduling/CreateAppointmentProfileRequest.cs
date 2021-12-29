using Scrips.Core.Models.Practice;
using System;

namespace Scrips.Core.Models.Scheduling
{
    public class CreateAppointmentProfileRequest
    {
        /// <summary>
        ///     Appointment profile id.
        /// </summary>

        public Guid? Id { get; set; }

        /// <summary>
        ///     Organization Id
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        ///     PractionerId Id
        /// </summary>
        public Guid PractionerId { get; set; }

        /// <summary>
        ///     PracticeId
        /// </summary>
        public Guid PracticeId { get; set; }

        /// <summary>
        ///     appoinement profile name
        /// </summary>
        public string ProfileName { get; set; }

        /// <summary>
        ///     Office | Home | Virtual
        /// </summary>
        public LocationModel Location { get; set; }

        /// <summary>
        ///     Can be less than start/end (e.g. estimate)
        /// </summary>
        public int? MinutesDuration { get; set; }

        /// <summary>
        /// </summary>
        public string Color { get; set; }


        /// <summary>
        ///     Coded reason this appointment is scheduled
        /// </summary>
        public ReasonCode ReasonCode { get; set; }

        /// <summary>
        ///     Coded reason this appointment is scheduled
        /// </summary>
        public AppointmentType AppointmentType { get; set; }

        /// <summary>
        ///     Id of the room to book an appointment.
        /// </summary>
        public Guid RoomId { get; set; }

        /// <summary>
        ///     Id of the billing profile.
        /// </summary>
        public Guid? BillingProfileId { get; set; }

        public BillingProfileDto BillingProfile { get; set; }

        /// <summary>
        /// </summary>
        public Guid ReferenceId { get; set; }
    }
}
