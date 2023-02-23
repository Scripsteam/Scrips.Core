namespace Scrips.Core.Models.Scheduling;

/// <summary>
/// Patient Appointment Payer Detail Model.
/// </summary>
public class PatientAppointmentPayerDetailModel
{
    /// <summary>
    /// Id.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Patient Id.
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Payer Id.
    /// </summary>
    public Guid PayerId { get; set; }

    /// <summary>
    /// Appointment Id.
    /// </summary>
    public Guid? AppointmentId { get; set; }

    /// <summary>
    /// Payer Code.
    /// </summary>
    public string PayerCode { get; set; }

    /// <summary>
    /// Is Confirmed.
    /// </summary>
    public bool IsConfirmed { get; set; }

    /// <summary>
    /// Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Image Url.
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    /// Is Active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Is Deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}