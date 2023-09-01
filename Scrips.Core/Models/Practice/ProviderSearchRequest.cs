using System.ComponentModel.DataAnnotations;

namespace Scrips.Core.Models.Practice;

public class ProviderSearchRequest
{
    public ProviderSearchRequest()
    {
        Location = new List<string>();
        Practitioners = new List<Guid>();
    }

    [Required]
    public Guid OrganizationId { get; set; }

    public Guid? PracticeId { get; set; }

    public List<string> Weekdays { get; set; } = new();

    /// <summary>
    /// Start date to filter the provider.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date to filter the provider.
    /// </summary>
    public DateTime EndDate { get; set; }

    public string SearchText { get; set; }

    public int SlotCount { get; set; }

    public bool SearchSlot { get; set; }

    public EnumAppointmentProfileType? AppointmentProfileType { get; set; }

    public IList<string> Location { get; set; }

    public bool IsInAppEnabled { get; set; } = false;

    public bool IsSearchFromDashboard { get; set; } = false;

    /// <summary>
    /// Practitioners ids.
    /// </summary>
    public List<Guid> Practitioners { get; set; }

    public Guid? SpecialitySkillId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}

public class ProviderSearchResponse
{
    public ProviderSearchResponse()
    {
        WorkingHours = new List<PracticeWorkingSlots>();
        Slots = new List<SlotResponse>();
        AppointmentProfile = new List<AppointmentProfileResponse>();
        practiceSlots = new List<PracticeSlot>();
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid WorkingHourId { get; set; }
    public Guid SpecialityId { get; set; }
    public int ServiceCategoryId { get; set; }
    public int ServiceTypeId { get; set; }
    public string Photo { get; set; }
    public string TimeZone { get; set; }
    public Guid OrganizationId { get; set; }
    public Speciality Speciality { get; set; }
    public ServiceCategory ServiceCategory { get; set; }
    public ServiceType ServiceType { get; set; }
    public IList<PracticeWorkingSlots> WorkingHours { get; set; }
    public IList<SlotResponse> Slots { get; set; }
    public IList<AppointmentProfileResponse> AppointmentProfile { get; set; }
    public IList<PracticeSlot> practiceSlots { get; set; }
    public int TotalSlots { get; set; }
    public bool AlreadyFiltered { get; set; }
    public IssueObject Issue { get; set; }
}

public class PracticeSlot
{
    public PracticeSlot()
    {
        AppointmentProfile = new AppointmentProfileResponse();
        Slots = new List<SlotResponse>();
        PracticeInfo = new PracticeInfo();
    }

    public Guid PracticeId { get; set; }
    public string PracticeName { get; set; }
    public AppointmentProfileResponse AppointmentProfile { get; set; }
    public IList<SlotResponse> Slots { get; set; }
    public PracticeInfo PracticeInfo { get; set; }
}

public class PracticeInfo
{
    public Guid Id { get; set; }
    public string PracticeName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string Office { get; set; }
    public string Photo { get; set; }
}

public class AvailableProviderRequest
{
    public AvailableProviderRequest()
    {
        Practitioners = new List<Guid>();
    }

    public Guid OrganizationId { get; set; }
    public Guid? PracticeId { get; set; }

    /// <summary>
    /// Start date to filter the provider.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date to filter the provider.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Practitioners ids.
    /// </summary>
    public List<Guid> Practitioners { get; set; }
}

public class AvailableProviderResponse
{
    public AvailableProviderResponse()
    {
        WorkingHours = new List<PracticeWorkingSlots>();
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Photo { get; set; }
    public Guid WorkingHourId { get; set; }
    public Guid SpecialityId { get; set; }
    public Speciality Speciality { get; set; }
    public IList<PracticeWorkingSlots> WorkingHours { get; set; }
}