using System;

namespace Scrips.Core.Models.Practice;

public class PracticeWorkingHoursDetailsModel
{
    public string WeekDayId { get; set; }
    public string StartHour { get; set; }
    public string StartMinute { get; set; }
    public string EndHour { get; set; }
    public string EndMinute { get; set; }
    public Guid PracticeId { get; set; }
}