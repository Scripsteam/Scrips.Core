using System.ComponentModel.DataAnnotations;

namespace Scrips.Core.Models.Practice;

public class PracticeSetupApiRequest
{
    public Guid PracticeProfileId { get; set; }
    //profile info
    [Required]
    public Guid OrganizationId { get; set; }
    public string PracticeImage { get; set; }
    [Required]
    public string PracticeName { get; set; }
    [Required]
    public string LicenseNumber { get; set; }
    [Required]
    public Guid LicenseAuthority { get; set; }
    public DateTime LicenseExpirationDate { get; set; }

    public string PracticeDescription { get; set; }
    public bool PrimaryPractice { get; set; }
    public bool BillingStatus { get; set; }

    public string BillingTaxId { get; set; }
    //contact/ details
    public ContactDetails ContactDetails { get; set; }

    //billing details
    public BillingDetails BillingDetails { get; set; }
}

public class ContactDetails
{
    [Required]
    public string Country { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string Address { get; set; }
    public string Office { get; set; }
    [Required]
    public string OfficePhone { get; set; }
    public string FaxNumber { get; set; }
    [Required]
    public string Latitude { get; set; }
    [Required]
    public string Longitude { get; set; }

}

public class BillingDetails
{
    public string Country { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string Office { get; set; }
    public string OfficePhone { get; set; }
    public string FaxNumber { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }

}