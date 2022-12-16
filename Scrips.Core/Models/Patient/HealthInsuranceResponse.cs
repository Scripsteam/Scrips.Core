using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Patient;

public class HealthInsuranceResponse
{
    public HealthInsuranceResponse()
    {
        BillingGroups = new List<PatientBillingGroupModel>();
    }
    public string HealthInsuranceId { get; set; }
    public string PatientId { get; set; }
    public ProviderInsuranceModel InsuranceProvider { get; set; }
    public string PolicyNumber { get; set; }
    public DateTime? EligiblityStartDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsDeleted { get; set; }
    public MasterResponse InsurerName { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public string FrontPhotoUrl { get; set; }
    public string BackPhotoUrl { get; set; }
    public string CopaymentType { get; set; }
    public bool IsReviewed { get; set; }
    public bool? IsVerified { get; set; }
    public List<PatientBillingGroupModel> BillingGroups { get; set; }
}