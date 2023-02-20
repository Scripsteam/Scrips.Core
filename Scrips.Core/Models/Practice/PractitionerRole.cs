namespace Scrips.Core.Models.Practice;

public class PractitionerRoleDto
{
    public Guid Id { get; set; }
    public Guid PractitionerId { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid DepartmentId { get; set; }
    public string Code { get; set; }
    public string TenantId { get; set; }

    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsArchived { get; set; }
}