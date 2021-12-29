using System;

namespace Scrips.Core.Models.Scheduling
{
    public class ProviderResponse
    {
        /// <summary>
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// </summary>
        public Speciality Speciality { get; set; }

        /// <summary>
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserId { get; set; }
    }
}
