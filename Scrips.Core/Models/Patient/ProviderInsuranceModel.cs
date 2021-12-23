using System.Collections.Generic;

namespace Scrips.Core.Models.Patient
{
    public class ProviderInsuranceModel
    {
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public string PayerName { get; set; }
        public string PayerCompanyId { get; set; }
        public string Office { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Note { get; set; }
        public List<ThirdPartyAdministrator> RelatedTpa { get; set; } = new();
    }
}
