using System;

namespace Scrips.Core.Models.Scheduling;

/// <summary>
///
/// </summary>
public class PatientAppointmentPayerDetailModel
{
    /// <summary>
    ///
    /// </summary>
    public Guid? Id { get; set; }
    /// <summary>
    ///
    /// </summary>
    public Guid PatientId { get; set; }
    /// <summary>
    ///
    /// </summary>
    public Guid PayerId { get; set; }
    /// <summary>
    ///
    /// </summary>
    public Guid? AppointmentId { get; set; }
    /// <summary>
    ///
    /// </summary>
    public string PayerCode { get; set; }
    /// <summary>
    ///
    /// </summary>
    public bool IsConfirmed { get; set; }
    /// <summary>
    ///
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    ///
    /// </summary>
    public string ImageUrl { get; set; }
    /// <summary>
    ///
    /// </summary>
    public bool IsActive { get; set; }
    /// <summary>
    ///
    /// </summary>
    public bool IsDeleted { get; set; }
}