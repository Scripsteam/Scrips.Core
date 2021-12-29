using System;

namespace Scrips.Core.Models.Scheduling
{
    public class ParticipantResponse
    {
        /// <summary>
        ///     System defined id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Whether this participant is required to be present at the meeting.
        /// </summary>
        public ParticipantRequired Required { get; set; }

        /// <summary>
        ///     Participation status of the actor.
        /// </summary>
        public ParticipationStatus Status { get; set; }

        /// <summary>
        ///     Role of participant in the appointment.
        /// </summary>
        public ParticipationType Type { get; set; }

        /// <summary>
        ///     Participation period of the actor.
        /// </summary>
        public PeriodResponse Period { get; set; }

        /// <summary>
        ///     An identifier for the target resource.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        ///     Person, Location, HealthcareService, or Device
        /// </summary>
        public Guid Reference { get; set; }

        /// <summary>
        ///     Plain text narrative that identifies the resource in addition to the resource reference.
        /// </summary>
        public string Display { get; set; }

        /// <summary>
        /// </summary>
        public string ReferenceType { get; set; }

        /// <summary>
        ///     Patient Data
        /// </summary>
        public PatientResponse Patient { get; set; }

        /// <summary>
        ///     Provider Data
        /// </summary>
        public ProviderResponse Provider { get; set; }
    }
}
