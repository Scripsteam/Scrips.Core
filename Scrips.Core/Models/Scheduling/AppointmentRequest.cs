using Scrips.Core.Models.Patient;
using System.ComponentModel.DataAnnotations;

namespace Scrips.Core.Models.Scheduling;

public class AppointmentsRequest
{
    /// <summary>
    ///     Day | Week | Month
    ///     Based on the selected <seealso cref="DateFilter" /> Start and End date will be calculated.
    /// </summary>
    [Required]
    public DateFilter Filter { get; set; } = DateFilter.Day;

    /// <summary>
    ///     Start date to filter the appointments
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    ///     End date to filter the appointments
    /// </summary>
    [Required]
    public DateTime EndDate { get; set; }

    /// <summary>
    ///     Id of the Organization.
    /// </summary>
    [Required]
    public Guid OrganizationId { get; set; }

    /// <summary>
    ///     Id of the Practice.
    /// </summary>
    [Required]
    public Guid PracticeId { get; set; }

    /// <summary>
    ///     Practitioners ids.
    /// </summary>
    public IList<Guid> Practitioners { get; set; } = new List<Guid>();

    /// <summary>
    ///     Search Text.
    /// </summary>
    public string SearchText { get; set; }

    /// <summary>
    ///     The overall status of the Appointment.
    /// </summary>
    public IList<Coding> Status { get; set; }

    /// <summary>
    ///     Appointment Modes to filter the appointments.
    /// </summary>
    public IList<Guid> Location { get; set; }

    /// <summary>
    ///     Appointment types to filter the appointments.
    /// </summary>
    public IList<Coding> Types { get; set; }

    /// <summary>
    ///     Page Number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    ///     Page Size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// </summary>
    public bool IsTimeSelected { get; set; } = false;

    /// <summary>
    /// </summary>
    public Guid? RecurringId { get; set; }
    public Guid? PatientId { get; set; }
    public bool? ShowActiveAppointmentOnly { get; set; }
}