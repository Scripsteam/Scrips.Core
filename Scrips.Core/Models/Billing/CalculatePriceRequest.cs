﻿namespace Scrips.Core.Models.Billing;

public class CalculatePriceRequest : BaseCalculatePrice
{
    public Guid? BillingProfileId { get; set; }
    public Guid LocationId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? SponsorId  {get; set;}
}