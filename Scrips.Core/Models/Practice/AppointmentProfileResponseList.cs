namespace Scrips.Core.Models.Practice;

public class AppointmentProfileResponseList
{
    public AppointmentProfileResponseList()
    {
        CreateAppointmentProfileRequest = new List<AppointmentProfileResponse>();
        IsDeleted = new List<Guid>();
    }

    public List<AppointmentProfileResponse> CreateAppointmentProfileRequest { get; set; }
    public IList<Guid> IsDeleted { get; set; }
    public IList<LocationModel> EnabledLocation { get; set; }
}