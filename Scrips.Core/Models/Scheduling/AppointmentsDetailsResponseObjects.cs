using Scrips.Core.Models.Patient;
using Scrips.Core.Models.Practice;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Scrips.Core.Models.Scheduling;

/// <summary>
///     Response model to get appointments details.
/// </summary>
public class AppointmentsDetailsResponse
{
    /// <summary>
    ///     Total number of appointments we have in database.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// </summary>
    public int TotalPage { get; set; }

    /// <summary>
    /// </summary>
    public int WaitingRoom { get; set; }

    /// <summary>
    /// </summary>
    public int Cancelled { get; set; }

    /// <summary>
    /// </summary>
    public int NoShow { get; set; }

    /// <summary>
    ///     List of Appointments.
    /// </summary>
    public ICollection<ListAppointmentResponse> Appointments { get; set; }

    /// <summary>
    /// </summary>
    public ICollection<BlockAppointmentRequest> BlockAppointment { get; set; }

    /// <summary>
    ///
    /// </summary>
    public AppointmentsDetailsResponseFacets Facets { get; set; }
}

public class AppointmentsDetailsResponseFacets
{
    /// <summary>
    ///     Participants involved in appointments
    /// </summary>
    public ICollection<ParticipantResponse> Participant { get; set; }

    /// <summary>
    ///     Practices involved in appointments
    /// </summary>
    public ICollection<PracticeResponse> Practice { get; set; }
}

public class ListAppointmentResponse : ListAppointmentResponseBase
{
    /// <summary>
    /// </summary>
    public bool IsBlock { get; set; }

    /// <summary>
    ///     Practice Id.
    /// </summary>
    public PracticeResponse Practice { get; set; }

    /// <summary>
    /// invoice object
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
    /// type of sponsor (Self,Insurance, Corporate)
    /// </summary>
    public string SponsorType { get; set; }

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
    /// Recurring flag
    /// </summary>
    public Guid? RecurringId { get; set; }

    /// <summary>
    /// Speciality
    /// </summary>
    public Speciality Specialty { get; set; }

    /// <summary>
    /// Speciality Id
    /// </summary>
    public Guid SpecialityId { get; set; }
}

public abstract class ListAppointmentResponseBase
{
    /// <summary>
    /// Constructor
    /// </summary>
    protected ListAppointmentResponseBase()
    {
        Participant = new HashSet<ParticipantResponse>();
    }

    /// <summary>
    ///     Id of the appointment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     When appointment is to take place
    /// </summary>
    public DateTime Start { get; set; }

    /// <summary>
    ///     When appointment is to conclude
    /// </summary>
    public DateTime End { get; set; }

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
    ///     Id of the room to book an appointment.
    /// </summary>
    public RoomResponse Room { get; set; }

    /// <summary>
    ///     Role of participant in the appointment
    /// </summary>
    public AppointmentType AppointmentType { get; set; }

    /// <summary>
    ///     The coded reason that this appointment is being scheduled. This is more clinical than administrative.
    /// </summary>
    public ReasonCode ReasonCode { get; set; }

    /// <summary>
    ///     Shown on a subject line in a meeting request, or appointment list
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     The slots that this appointment is filling
    /// </summary>
    public SlotResponse Slot { get; set; }

    /// <summary>
    ///     Additional comments
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    ///     Participants involved in appointment
    /// </summary>
    public ICollection<ParticipantResponse> Participant { get; set; }

    /// <summary>
    /// </summary>
    public CreateAppointmentProfileRequest AppointmentProfile { get; set; }

    /// <summary>
    /// </summary>
    public IList<FlagResponse> FlagList { get; set; }

}

public class BlockAppointmentRequest
{
    /// <summary>
    ///     Block appointment Id
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// </summary>
    [Required]
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// </summary>
    [Required]
    public ProviderResponse Practitioner { get; set; }

    /// <summary>
    /// </summary>
    [Required]
    public Guid PracticeId { get; set; }

    /// <summary>
    /// </summary>
    [Required]
    public string Note { get; set; }

    /// <summary>
    /// </summary>
    [Required]
    public DateTime BlockDate { get; set; }

    /// <summary>
    ///     Appointment template StartTime
    /// </summary>
    [Required]
    public BlockTime StartTime { get; set; }

    /// <summary>
    ///     Appointment template EndTime
    /// </summary>
    [Required]
    public BlockTime EndTime { get; set; }

    /// <summary>
    /// </summary>
    public bool UnBlock { get; set; }

    /// <summary>
    ///
    /// </summary>
    public SlotResponse Slot { get; set; }

    /// <summary>
    /// this property is merged from the startTime
    /// </summary>
    public DateTime StartDate { get; set; }
    /// <summary>
    /// this property is merged from the endTime
    /// </summary>
    public DateTime EndDate { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? RecurringId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public BlockRecurrenceCriteria Recurring { get; set; }
}

public class BlockTime
{
    /// <summary>
    ///     Start time
    /// </summary>
    [Required]
    public int Hour { get; set; }

    /// <summary>
    ///     End time
    /// </summary>
    [Required]
    public int Minute { get; set; }

    /// <summary>
    ///     Format AM/PM
    /// </summary>
    [Required]
    public string Format { get; set; }
}

public class BlockRecurrenceCriteria
{
    /// <summary>
    /// 
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// ProviderId
    /// </summary>
    public Guid ProviderId { get; set; }

    /// <summary>
    /// PracticeId
    /// </summary>
    public Guid PracticeId { get; set; }

    /// <summary>
    /// AppointmentProfileId
    /// </summary>
    public Guid AppointmentProfileId { get; set; }

    /// <summary>
    /// StartDate
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// EndDate
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// RepeatEvery
    /// </summary>
    public int RepeatEvery { get; set; }

    /// <summary>
    /// RepeatEvery
    /// </summary>
    public int NumberOfRepeat { get; set; } = 0;

    /// <summary>
    /// Period 
    /// </summary>
    public FuRequestFollowUp Period { get; set; }

    /// <summary>
    /// Days
    /// </summary>
    public List<DayOfWeek> Days { get; set; }
}

/// <summary>
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FuRequestFollowUp
{
    /// <summary>
    /// </summary>
    Days,

    /// <summary>
    /// </summary>
    Weeks,

    /// <summary>
    /// </summary>
    Months,

    /// <summary>
    /// </summary>
    Years
}