using System;

namespace Scrips.Core.Models.Identity;

public class TenantDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string AdminEmail { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime ValidUpto { get; set; }
}