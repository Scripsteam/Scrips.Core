using System;

namespace Scrips.Core.Models.Practice;

public class LocationModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsArchived { get; set; }
    public string ImageUrl { get; set; }
}