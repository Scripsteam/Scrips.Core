namespace Scrips.Core.Models.Practice;

/// <summary>
/// Appointment profile response.
/// </summary>
public class AppointmentProfileResponse
{
    /// <summary>
    /// Appointment profile id.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Organization Id.
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>
    /// PractitionerId Id.
    /// </summary>
    public Guid PractionerId { get; set; }

    /// <summary>
    /// PracticeId.
    /// </summary>
    public Guid PracticeId { get; set; }

    /// <summary>
    /// Appointment profile name.
    /// </summary>
    public string ProfileName { get; set; }

    /// <summary>
    /// Office | Home | Video.
    /// </summary>
    public LocationModel Location { get; set; }

    /// <summary>
    /// Can be less than start/end (e.g. estimate).
    /// </summary>
    public int? MinutesDuration { get; set; }

    /// <summary>
    /// Color of the appointment.
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// Coded reason this appointment is scheduled.
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

    /// <summary>
    /// Billing profile.
    /// </summary>
    public BillingProfileDto BillingProfile { get; set; }

    /// <summary>
    /// Id of the appointment type.
    /// </summary>
    public bool InAppBooking { get; set; }

    /// <summary>
    /// is default profile.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// is updated profile.
    /// </summary>
    public bool IsUpdated { get; set; }

    /// <summary>
    /// is preferred profile.
    /// </summary>
    public bool? IsPreferred { get; set; }

    /// <summary>
    /// </summary>
    public Speciality Speciality { get; set; }

    /// <summary>
    /// </summary>
    public Guid SpecialityId { get; set; }
}