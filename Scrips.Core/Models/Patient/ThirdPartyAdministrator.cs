using System;

namespace Scrips.Core.Models.Patient;

public class ThirdPartyAdministrator
{
    public Guid OrganizationId { get; set; }
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string CompanyName { get; set; }
    public string Note { get; set; }
    public string ImageUrl { get; set; }
    public string Website { get; set; }
    public string PhoneNumber { get; set; }
    public string Office { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public bool IsCustom { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Guid UpdatedById { get; set; }
}