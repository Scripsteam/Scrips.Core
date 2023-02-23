namespace Scrips.Core.Models.Scheduling;

public class AppointmentPagedResponse
{
    public int CurrentPage { get; set; }
    public int Count { get; set; }
    public List<AppointmentResponse> AppointmentList { get; set; }
}