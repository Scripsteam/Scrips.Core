using System;

namespace Scrips.Core.Models.Provider;

public class AddressModel
{
    public Guid? Id { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string PostalCode { get; set; }
    public string Lat { get; set; }
    public string Lang { get; set; }
}