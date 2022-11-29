namespace Scrips.Core.Models.Scheduling;

/// <summary>
///     This value set defines an example set of codes of service-types.
/// </summary>
public class ServiceType
{
    /// <summary>
    ///     Code defined by a terminology system
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    ///     An explanation of the meaning of the concept
    /// </summary>
    public string Definition { get; set; }

    /// <summary>
    ///     A human language representation of the concept as seen/selected/uttered by the user who entered the data
    ///     and/or which represents the intended meaning of the user.
    /// </summary>
    public string Display { get; set; }
}