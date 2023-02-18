namespace Scrips.Core.Models.Practice;

public class PractitionerRole
{
    public Guid Id { get; set; }
    public Guid PractitionerId { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid DepartmentId { get; set; }
    public string Code { get; set; }
    public string TenantId { get; set; }
}