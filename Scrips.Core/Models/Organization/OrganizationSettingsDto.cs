using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Organization;

public class OrganizationSettingsDto
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string TimeZone { get; set; }
    public string MedicationSystem { get; set; }
    public bool? HasPatientCommunication { get; set; }
    public string TenantId { get; set; }
    public string Address { get; set; }
    public string AddressLatitude { get; set; }
    public string AddressLongitude { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Office { get; set; }
    public string OfficePhone { get; set; }
    public string Fax { get; set; }
    public string PoBox { get; set; }
    public string BillingTaxId { get; set; }
    public string BillingAddress { get; set; }
    public string BillingAddressLatitude { get; set; }
    public string BillingAddressLongitude { get; set; }
    public string BillingCountry { get; set; }
    public string BillingCity { get; set; }
    public string BillingOffice { get; set; }
    public string BillingOfficePhone { get; set; }
    public string BillingFax { get; set; }
    public string BillingPoBox { get; set; }
    public List<string> Contacts { get; set; }
}