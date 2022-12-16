namespace Scrips.Core.Models.Scheduling;

public class PracticeResponse
{
    /// <summary>
    /// Unique ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Practice Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Practice Phone
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// Practice Address
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Practice Country
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// Practice City
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Practice Office
    /// </summary>
    public string Office { get; set; }

    /// <summary>
    /// Practice Fax
    /// </summary>
    public string FaxNumber { get; set; }

    /// <summary>
    /// Practice Image/ Photo
    /// </summary>
    public string Photo { get; set; }

    /// <summary>
    /// Latitude
    /// </summary>
    public string Latitude { get; set; }

    /// <summary>
    /// Longitude
    /// </summary>
    public string Longitude { get; set; }

    public string NabidhAssigningAuthority { get; set; }

    public string LicenseNumber { get; set; }

    public Guid LicenseAuthority { get; set; }
}