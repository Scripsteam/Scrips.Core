namespace Scrips.BaseDbContext;

/// <summary>
/// Attribute to flag properties and/or entities that there values considered to be sensetive and not tracked in audits
/// </summary>
/// <example>
/// Mark any applicable property with:
///		<code>
///			[MaskValueAudit]
///			public string Password {get; set;}
///		</code>
/// </example>
public class MaskValueAuditAttribute : Attribute
{
    public MaskValueAuditAttribute()
    { }
}