using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Scheduling;

public class RecurringDto
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int RepeatEvery { get; set; }
    public int NumberOfRepeat { get; set; }
    public int Occurrence { get; set; }
    public string Period { get; set; }
    public List<string> Days { get; set; }
}