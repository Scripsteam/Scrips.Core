using System;

namespace Scrips.Core.Models.Billing
{
    public class PreCalculatePriceRequest : BaseCalculatePrice
    {
        public Guid InvoiceId { get; set; }
    }
}
