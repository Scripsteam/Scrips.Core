using Scrips.Core.Models.Patient;
using System.Collections.Generic;

namespace Scrips.Core.Models.Scheduling
{
    public class Sponsor
    {
        public Sponsor()
        {
            RelatedTpa = new List<ThirdPartyAdministrator>();
        }
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// image URL
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// payer name
        /// </summary>
        public string PayerName { get; set; }
        /// <summary>
        /// payer company id
        /// </summary>
        public string PayerCompanyId { get; set; }
        /// <summary>
        /// office
        /// </summary>
        public string Office { get; set; }
        /// <summary>
        /// address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// website
        /// </summary>
        public string Website { get; set; }
        /// <summary>
        /// email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// phone number
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// note
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// TPA
        /// </summary>
        public List<ThirdPartyAdministrator> RelatedTpa { get; set; }
    }
}
