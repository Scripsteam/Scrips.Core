namespace Scrips.Core.Models.Scheduling;

public class RoomResponse
{
    /// <summary>
    /// Unique ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Room Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// False when the room is closed (Practice Management: inactive or deleted). The apps show "(closed)" next to the
    /// name. True by default: a room that is not known to be closed is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}