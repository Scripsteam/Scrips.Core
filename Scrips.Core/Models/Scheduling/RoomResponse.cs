using System;

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
}