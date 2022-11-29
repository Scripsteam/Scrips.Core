using Scrips.Core.Models.Scheduling;
using System;

namespace Scrips.Core.Models.Patient;

public class GetPatientFlagResponse
{
    public Guid? identifier { get; set; }
    public Guid? subject { get; set; }
    public Guid? encounter { get; set; }
    public Guid? author { get; set; }
    public string dataAbsentReason { get; set; }
    public DateTime? startDate { get; set; }
    public DateTime? endDate { get; set; }
    public bool? isDeleted { get; set; }
    public string CategoryName { get; set; }
    public FlagResponse FlagResponse { get; set; }
    public DateTime? createdDate { get; set; }
    public Guid? CreatedBy { get; set; }
}