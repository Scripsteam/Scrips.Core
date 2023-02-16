using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrips.Core.Models.Organization;

public class OrganizationV1Dto
{
    public Guid Id { get; set; }
    public string TenantId { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsActive { get; set; }
    public bool IsArchived { get; set; }
    public string LicenseType { get; set; }
    public string LicenseNumber { get; set; }
    public DateTime? LicenseExpirationDate { get; set; }
    public string LicenseIssuer { get; set; }
    public string LogoUrl { get; set; }
    public string FrontLicenseIdURL { get; set; }
    public string BackLicenseIdURL { get; set; }
    public string BrandingUrl { get; set; }
    public string NameArabic { get; set; }
    public string Jurisdiction { get; set; }
    public Guid? OwnerId { get; set; }

}
