using Scrips.Core.Models.Patient;
using Scrips.Core.Models.Practice;
using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Scheduling
{
    /// <summary>
    ///     Appointment response.
    /// </summary>
    public class AppointmentResponse
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public AppointmentResponse()
        {
            Participant = new HashSet<ParticipantResponse>();
            CompletedAppointmentResponses = new CompletedAppointmentResponse();
            BillingTotal = new BillingTotal();
            AddedIndividualServices = new List<InvoiceDetailsDto>();
        }

        /// <summary>
        ///     Id of the appointment.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     The overall status of the Appointment.
        ///     Each of the participants has their own participation status which indicates their involvement in the process,
        ///     however this status indicates the shared status.
        /// </summary>
        public AppointmentStatus Status { get; set; }

        /// <summary>
        ///     Office | Home | Video
        /// </summary>
        public LocationModel Location { get; set; }

        /// <summary>
        ///     Organization Id
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        ///     Practice Id
        /// </summary>
        public Guid PracticeId { get; set; }


        /// <summary>
        ///     Practice Id
        /// </summary>
        public PracticeResponse Practice { get; set; }

        /// <summary>
        ///     Id of the room to book an appointment.
        /// </summary>
        public Guid RoomId { get; set; }

        /// <summary>
        ///     Id of the room to book an appointment.
        /// </summary>
        public RoomResponse Room { get; set; }

        /// <summary>
        ///     The coded reason for the appointment being canceled
        /// </summary>
        public AppointmentCancellationReason CancelationReason { get; set; }

        /// <summary>
        ///     A broad categorization of the service that is to be performed during this appointment
        /// </summary>
        public ServiceCategory ServiceCategory { get; set; }

        /// <summary>
        ///     The specific service that is to be performed during this appointment
        /// </summary>
        public ServiceType ServiceType { get; set; }

        /// <summary>
        ///     The specialty of a practitioner that would be required to perform the service requested in this appointment
        /// </summary>
        public Speciality Specialty { get; set; }

        /// <summary>
        ///     Role of participant in the appointment
        /// </summary>
        public AppointmentType AppointmentType { get; set; }

        /// <summary>
        ///     The coded reason that this appointment is being scheduled. This is more clinical than administrative.
        /// </summary>
        public ReasonCode ReasonCode { get; set; }

        /// <summary>
        ///     Used to make informed decisions if needing to re-prioritize
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        ///     Shown on a subject line in a meeting request, or appointment list
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Additional information to support the appointment
        /// </summary>
        public string SupportingInformation { get; set; }

        /// <summary>
        ///     When appointment is to take place
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        ///     When appointment is to conclude
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        ///     Can be less than start/end (e.g. estimate)
        /// </summary>
        public int? MinutesDuration { get; set; }

        /// <summary>
        ///     Can be less than start/end (e.g. estimate)
        /// </summary>
        public int ReferenceId { get; set; }

        /// <summary>
        ///     The slots that this appointment is filling
        /// </summary>
        public SlotResponse Slot { get; set; }


        /// <summary>
        ///     The date that this appointment was initially created
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        ///     Additional comments
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        ///     Detailed information and instructions for the patient
        /// </summary>
        public string PatientInstruction { get; set; }

        /// <summary>
        ///     Participants involved in appointment
        /// </summary>
        public ICollection<ParticipantResponse> Participant { get; set; }

        /// <summary>
        ///     Potential date/time interval(s) requested to allocate the appointment within
        /// </summary>
        public PeriodResponse RequestedPeriod { get; set; }

        /// <summary>
        /// </summary>
        public CreateAppointmentProfileRequest AppointmentProfile { get; set; }

        /// <summary>
        /// </summary>
        public CreateReminderProfileRequest ReminderProfile { get; set; }

        /// <summary>
        /// </summary>
        public IList<FlagResponse> FlagList { get; set; }

        /// <summary>
        ///     Questionnaire Form Id
        /// </summary>
        public QuestionnaireRequest QuestionnaireForm { get; set; }

        /// <summary>
        /// </summary>
        public CompletedAppointmentResponse CompletedAppointmentResponses { get; set; }

        /// <summary>
        ///     the person who updated the appointment
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// </summary>
        public string CreatedPerson { get; set; }

        /// <summary>
        /// </summary>
        public string UpdatedPerson { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PatientAppointmentPayerDetailModel PatientAppointmentPayerDetail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SponsorType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BillingTotal BillingTotal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AppointmentInvoiceModel Invoice { get; set; }

        /// <summary>
        /// sponsor
        /// </summary>
        public Sponsor Sponsor { get; set; }

        /// <summary>
        /// policy no
        /// </summary>
        public string PolicyNo { get; set; }

        /// <summary>
        /// Residential Address Details
        /// </summary>
        public PatientAddressListResponse ResidentialAddress { get; set; }

        /// <summary>
        /// CallBack Contract Details
        /// </summary>
        public EmergencyContactResponse CallBackContract { get; set; }

        /// <summary>
        /// Payment Link
        /// </summary>
        public string PaymentLink { get; set; }

        /// <summary>
        /// Added Services
        /// </summary>
        public List<InvoiceDetailsDto> AddedIndividualServices { get; set; }

    }

}
