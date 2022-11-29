using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Practice
{
    public class PracticeWorkingHourNew
    {
        public int WeekdayId { get; set; }
        public string StartWeekDay { get; set; }
        public string EndWeekDay { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public class PracticeWorkingHourSlotList
    {
        public Guid PracticeId { get; set; }
        public Guid WorkingHoursId { get; set; }
        public int Type { get; set; }
        public string ProfileName { get; set; }
        public string PracticeName { get; set; }
        public Guid RoomId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PracticeImageUrl { get; set; }
        public List<LocationModel> Locations { get; set; }
        public List<PracticeWorkingSlots> PracticeWorkingSlots { get; set; }

    }

    public class PracticeWorkingSlots
    {
        public int WeedayId { get; set; }
        public string StartWeekDay { get; set; }
        public string EndWeekDay { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool? DayOff { get; set; }

    }

    public class PracticeSetupList
    {
        public PracticeSetupList()
        {
            WorkingHours = new List<WorkingHours>();
        }
        public string PracticeName { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid PracticeSetupId { get; set; }

        public string Address { get; set; }

        public string PracticeImageURL { get; set; }

        public bool IsPrimary { get; set; }
        public List<WorkingHours> WorkingHours { get; set; }
        public List<LocationModel> Locations { get; set; }

    }

    public class WorkingHours
    {
        public int WeedayId { get; set; }
        public string StartWeekDay { get; set; }
        public string EndWeekDay { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public class PracticeWorkingHour
    {
        public string StartWeekDay { get; set; }
        public string EndWeekDay { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
