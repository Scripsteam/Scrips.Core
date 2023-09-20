using Scrips.Core.Models.Practice;

namespace Scrips.Core.Models.Scheduling
{
    public class AppointmentPartDetails
    {
        public Guid Id { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public AppointmentStatus Status { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public LocationModel Location { get; set; }
        public ReasonCode ReasonCode { get; set; }
        public PatientDetails Patient { get; set; }
    }

    public class PatientDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
