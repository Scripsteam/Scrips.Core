namespace Scrips.Core.Models.Scheduling;

public class ProviderResponse
{
    /// <summary>
    /// Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Photo.
    /// </summary>
    public string Photo { get; set; }

    /// <summary>
    /// Speciality.
    /// </summary>
    public Speciality Speciality { get; set; }

    /// <summary>
    /// Time Zone.
    /// </summary>
    public string TimeZone { get; set; }

    /// <summary>
    /// Organization Id.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// User Id.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// First Name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Last Name.
    /// </summary>
    public string LastName { get; set; }
}