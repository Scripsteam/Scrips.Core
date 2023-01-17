using System.Collections.Generic;

namespace Scrips.Core.Models.Practice;

public class PracticeDetailsResponse
{
    public PracticeSetupDetails PracticeSetupDetail { get; set; }
    public List<PracticeWorkingHourNew> PracticeWorkingHours { get; set; }
    public List<PracticeHoliday> PracticeHoliday { get; set; }
    public List<UpdateExamRoomApiRequest> PracticeRoomDetail { get; set; }
}