using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Scheduling
{
    /// <summary>
    ///     Request to create reminder profile for appointment
    /// </summary>
    public class CreateReminderProfileRequest
    {
        public CreateReminderProfileRequest()
        {
            ReminderProfileType = new List<ReminderProfileTypeRequest>();
        }
        /// <summary>
        ///     Id be reminder profile is old
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        ///     organization id
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        ///     practice id
        /// </summary>
        public Guid PracticeId { get; set; }

        /// <summary>
        /// </summary>
        public Guid? PractitionerId { get; set; }

        /// <summary>
        ///     profile name
        /// </summary>
        public string ProfileName { get; set; }

        /// <summary>
        ///     Custom notes
        /// </summary>
        public string CustomNote { get; set; }

        /// <summary>
        ///     Reminider profile type
        /// </summary>
        public List<ReminderProfileTypeRequest> ReminderProfileType { get; set; }
    }
}
