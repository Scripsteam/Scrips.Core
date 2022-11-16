using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Practice
{
    /// <summary>
    /// 
    /// </summary>
    public class AppointmentProfileResponse
    {
        /// <summary>
        /// Appointment profile id.
        /// </summary>

        public Guid? Id { get; set; }

        /// <summary>
        /// Organization Id
        /// </summary>
        public Guid? OrganizationId { get; set; }

        /// <summary>
        /// PractionerId Id
        /// </summary>
        public Guid PractionerId { get; set; }
        /// <summary>
        /// PracticeId
        /// </summary>
        public Guid PracticeId { get; set; }

        /// <summary>
        /// appoinement profile name
        /// </summary>
        public string ProfileName { get; set; }

        /// <summary>
        /// Office | Home | Video
        /// </summary>
        public LocationModel Location { get; set; }

        /// <summary>
        /// Can be less than start/end (e.g. estimate)
        /// </summary>
        public int? MinutesDuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Color { get; set; }


        /// <summary>
        /// Coded reason this appointment is scheduled
        /// </summary>
        public ReasonCode ReasonCode { get; set; }

        /// <summary>
        /// Coded reason this appointment is scheduled
        /// </summary>
        public AppointmentType AppointmentType { get; set; }

        /// <summary>
        /// Id of the room to book an appointment.
        /// </summary>
        public Guid RoomId { get; set; }

        /// <summary>
        /// Id of the billing profile.
        /// </summary>
        public Guid? BillingProfileId { get; set; }

        public BillingProfileDto BillingProfile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool InAppBooking { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsUpdated { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsPreferred { get; set; }


    }

    public class AppointmentProfileResponseList
    {
        public AppointmentProfileResponseList()
        {
            CreateAppointmentProfileRequest = new List<AppointmentProfileResponse>();
            IsDeleted = new List<Guid>();
        }

        public IList<AppointmentProfileResponse> CreateAppointmentProfileRequest { get; set; }
        public IList<Guid> IsDeleted { get; set; }
    }

    public class AppointmentProfileModel
    {
        public string ProfileName { get; set; }
        public string Location { get; set; }
        public string Color { get; set; }
        public string AppointmentTypeId { get; set; }
        public int Duration { get; set; }
    }
}
