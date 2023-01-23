using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Practice;

public class ReminderResponse
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid PracticeId { get; set; }
    public Guid? PractitionerId { get; set; }
    public string? ProfileName { get; set; }
    public string? CustomNote { get; set; }
    public bool EnableInAppBooking { get; set; }
    public bool IsDefault { get; set; }
    public List<ReminderProfileType>? ReminderProfileType { get; set; }
}