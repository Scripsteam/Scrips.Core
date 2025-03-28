using System.ComponentModel.DataAnnotations;

namespace Scrips.Core.Models.Practice;

/// <summary>
///  Request body for appointment Profile
/// </summary>
public class CreateAppointmentProfileRequest
{
    /// <summary>
    /// Appointment profile id.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Organization Id.
    /// </summary>
    [Required]
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// PracticeId.
    /// </summary>
    [Required]
    public Guid PracticeId { get; set; }

    /// <summary>
    /// Practitioner Id.
    /// </summary>
    [Required]
    public Guid PractionerId { get; set; }

    /// <summary>
    /// appointment profile name.
    /// </summary>
    public string ProfileName { get; set; }

    /// <summary>
    /// Office | Home | Virtual.
    /// </summary>
    [Required]
    public LocationModel Location { get; set; }

    /// <summary>
    /// Can be less than start/end (e.g. estimate).
    /// </summary>
    public int? MinutesDuration { get; set; }

    /// <summary>
    /// Office | Home | Video.
    /// </summary>
    [Required]
    public string Color { get; set; }

    /// <summary>
    /// Coded reason this appointment is scheduled.
    /// </summary>
    public ReasonCode ReasonCode { get; set; }

    /// <summary>
    /// Coded reason this appointment is scheduled.
    /// </summary>
    [Required]
    public AppointmentType AppointmentType { get; set; }

    /// <summary>
    /// Id of the room to book an appointment.
    /// </summary>
    [Required]
    public Guid RoomId { get; set; }

    /// <summary>
    /// Id of the billing profile.
    /// </summary>
    public Guid? BillingProfileId { get; set; }

    /// <summary>
    /// In App Booking.
    /// </summary>
    public bool InAppBooking { get; set; }

    /// <summary>
    /// Is updated.
    /// </summary>
    public bool IsUpdated { get; set; }

    /// <summary>
    /// Is preferred.
    /// </summary>
    public bool IsPreferred { get; set; }

    /// <summary>
    /// SpecialtyId.
    /// </summary>
    public Guid SpecialityId { get; set; }
}

/// <summary>
/// The coded reason that this appointment is being scheduled. This is more clinical than administrative.
/// </summary>
public class AppointmentType
{
    /// <summary>
    /// Code defined by a terminology system.
    /// </summary>
    [Required]
    [Key]
    public string Code { get; set; }

    /// <summary>
    /// An explanation of the meaning of the concept.
    /// </summary>
    [MaxLength(500)]
    public string Definition { get; set; }

    /// <summary>
    /// A human language representation of the concept as seen/selected/uttered by the user who entered the data
    /// and/or which represents the intended meaning of the user.
    /// </summary>
    // [Required]
    [MaxLength(500)]
    public string Display { get; set; }
}

public class CreateAppointmentProfileRequestList
{
    public IList<CreateAppointmentProfileRequest> CreateAppointmentProfileRequest { get; set; }
    public IList<Guid> IsDeleted { get; set; }
}