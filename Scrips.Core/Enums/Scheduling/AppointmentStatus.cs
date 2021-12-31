using System.Text.Json.Serialization;

namespace Scrips.Core.Enums.Scheduling
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AppointmentStatus
    {
        Proposed,
        Pending,
        Booked,
        Arrived,
        Fulfilled,
        Cancelled,
        NoShow,
        EnteredInError,
        CheckedIn,
        Waitlist,
        Confirmed,
        Seen,
    }
}
