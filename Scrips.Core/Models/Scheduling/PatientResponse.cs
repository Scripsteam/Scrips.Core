using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Scrips.Core.Models.Scheduling
{

    /// <summary>
    ///     Patient Response
    /// </summary>
    public class PatientResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public PatientResponse()
        {
            GuardianDetails = new GuardianDetails();
        }

        /// <summary>
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// </summary>
        public string Mrn { get; set; }

        /// <summary>
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// </summary>
        public string GenderName { get; set; }

        /// <summary>
        /// </summary>
        public Guid Gender { get; set; }

        /// <summary>
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IdentificationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GuardianDetails GuardianDetails { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Age { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOnboarding { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name => FirstName.Trim() +
                              $"{(!string.IsNullOrWhiteSpace(MiddleName) ? " " + MiddleName.Trim() : string.Empty)}" + " " + LastName.Trim();
        /// <summary>
        /// 
        /// </summary>
        public bool IsInsuranceAdded { get; set; }
        
        [JsonExtensionData]
        public IDictionary<string, JToken> Extensions { get; set; } = new Dictionary<string, JToken>();

    }

}
