﻿namespace Scrips.Core.Models.Scheduling
{
    public class SchedulingActionObject
    {
        public ActionStatus Status { get; set; }
        public object Data { get; set; }
        public List<AppointmentPartialDetails> Blockers { get; set; }
    }

    public enum ActionStatus {  Success, Failure }

}
