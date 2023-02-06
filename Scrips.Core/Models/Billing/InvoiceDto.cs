using Scrips.Core.Models.Scheduling;

namespace Scrips.Core.Models.Billing;

public class InvoiceDto
{
    public Guid? Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? InvoiceStatus { get; set; }
    public Guid? CancelledReason { get; set; }
    public string? SponsorType { get; set; }
    public Guid SponsorId { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public string? ProviderName { get; set; }
    public string? ProviderImageUrl { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid PracticeId { get; set; }
    public string? PracticeName { get; set; }
    public string? PracticeImageUrl { get; set; }
    public string TrnNumber { get; set; }
    public Guid LocationId { get; set; }
    public Guid? BillingProfileId { get; set; }
    public Guid? AppointmentProfileId { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? Note { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid? EncounterId { get; set; }
    public string? EncounterNumber { get; set; }
    public DateTime? EncounterDate { get; set; }
    public DateTime? DraftedOn { get; set; }
    public DateTime? CreatedOn { get; set; }
    public Guid? CreatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public Guid? UpdatedById { get; set; }
    public bool? IsArchived { get; set; }
    public double GrossTotal { get; set; }
    public double DiscountTotal { get; set; }
    public double NetTotal { get; set; }
    public double PatientShareTotal { get; set; }
    public double SponsorAmountTotal { get; set; }
    public double ExcludedTotal { get; set; }
    public double PatientPayable { get; set; }
    public double Deposited { get; set; }
    public double PatientOutstanding { get; set; }
    public double SponsorOutstanding { get; set; }
    public double ManagementDiscount { get; set; }
    public string? PatientPaymentStatus { get; set; }
    public string? SponsorDetails { get; set; }
    public DateTime? EligibilityStartDate { get; set; }
    public DateTime? EligibilityEndDate { get; set; }
    public List<InvoiceDetailsDto>? Services { get; set; }
    public PracticeResponse? Practice { get; set; }
    public PatientResponse? Patient { get; set; }
    public ProviderResponse? Provider { get; set; }
    public Sponsor? Sponsor { get; set; }
    public ClaimForInvoiceDto? Claim { get; set; }
    public OrganizationDto Organization { get; set; }
    public string? PolicyNo { get; set; }
    public List<AssessmentPlanCondition>? DiagnosisList { get; set; }
    public CreatePatientAssessmentPlanRequest? AssessmentPlan { get; set; }
    public List<BaseErrorDto>? Errors { get; set; }
}

public class BaseErrorDto
{
    public string? Header { get; set; }
    public string? Message { get; set; }
}

public class CreatePatientAssessmentPlanRequest
{
    public Guid? Identifier { get; set; }
    public Guid? Encounter { get; set; }
    public Guid? Subject { get; set; }
    public Guid? Performer { get; set; }
    public bool? IsOrderSign { get; set; }
    public string? Notes { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? OrderDate { get; set; }
    public List<AssessmentPlanCondition>? AssessmentPlanCondition { get; set; }
    public List<AssessmentPlanProcedures>? AssessmentPlanProcedures { get; set; }
    public List<AssessmentPlanSnomed>? AssessmentPlanSnomed { get; set; }
    public List<AssessmentPlanMedication>? AssessmentPlanMedication { get; set; }
    public List<RadiologyOrderRequest>? RadiologyOrderRequest { get; set; }
    public bool? HasOrderSent { get; set; }
}

public class MedicationCode
{
    public string? System { get; set; }
    public string? Code { get; set; }
    public string? Display { get; set; }
}

public class RadiologyOrderRequest
{
    public Guid? Id { get; set; }
    public Guid? AssessmentPlanId { get; set; }
    public Guid? ParentId { get; set; }
    public ChildMedicalCode ChildMedicalCode { get; set; }
    public string? Status { get; set; }
    public string? Intent { get; set; }
    public string? CategoryCode { get; set; }
    public string? CategoryName { get; set; }
    public string? Priority { get; set; }
    public TestCode Code { get; set; }
    public Guid? Subject { get; set; }
    public Guid? Performer { get; set; }
    public Guid? Encounter { get; set; }
    public string? Note { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
    public List<OrderDx> OrderDx { get; set; }
    public OrderLabs OrderLabs { get; set; }
}

public class AssessmentPlanMedication
{
    public Guid? Identifier { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? Encounter { get; set; }
    public bool? IsInteractionAlert { get; set; }
    public Guid? AssessmentPlanId { get; set; }
    public ChildMedicalCode ChildMedicalCode { get; set; }
    public MedicationCode MedicationCode { get; set; }
    public string? Frequency { get; set; }
    public string? Duration { get; set; }
    public string? DurationUnit { get; set; }
    public string? Instruction { get; set; }
    public string? Sig { get; set; }
    public int? Dispense { get; set; }
    public int? Refill { get; set; }
    public bool? Substitution { get; set; }
    public Guid? PharmacyId { get; set; }
    public string? PharmacyName { get; set; }
    public Guid? Subject { get; set; }
    public Guid? Performer { get; set; }
    public string? Notes { get; set; }
    public DateTime? OnSetDate { get; set; }
    public DateTime? OrderDate { get; set; }
    public bool? IsDeleted { get; set; }
    public List<MedicationOccurenceRequest> OccurenceRequest { get; set; }
    public List<VidalInteractionAlert> VidalAlertRequest { get; set; }
    public List<RxDiagnosisCodesRequest> RxDiagnosisCodesRequest { get; set; }
}

public class VidalInteractionAlert
{
    public string? Title { get; set; }
    public string? Id { get; set; }
    public string? AlertType { get; set; }
    public string? Description { get; set; }
    public string? Severity { get; set; }
    public string? TriggeredBy { get; set; }
    public string? SubType { get; set; }
    public string? TriggeredDetail { get; set; }
    public bool? IsAcknowledged { get; set; }
}

public class RxDiagnosisCodesRequest
{
    public Guid? Id { get; set; }
    public Guid? PatientRxId { get; set; }
    public bool? IsActive { get; set; }
    public DxCode DxCode { get; set; }
    public bool? IsDeleted { get; set; }
}

public class DxCode
{
    public Guid? DxId { get; set; }
    public string? Code { get; set; }
    public string? DisplayName { get; set; }
    public string? System { get; set; }
}


public class MedicationOccurenceRequest
{
    public Guid? Id { get; set; }
    public Guid? MedicationPlanId { get; set; }
    public string? Count { get; set; }
    public int? CountMax { get; set; }
    public int? DaysInterval { get; set; }
    public int? Period { get; set; }
    public int? PeriodMax { get; set; }
    public string? PeriodUnit { get; set; }
    public DateTime? OccurenceTiming { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
}

public class OrderLabs
{
    public Guid? Id { get; set; }
    public Guid? ServiceOrderId { get; set; }
    public Guid? LabId { get; set; }
    public string? LabName { get; set; }
    public string? LabAddress { get; set; }
    public string? LabContactNo { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
}


public class OrderDx
{
    public Guid? Id { get; set; }
    public Guid? ServiceOrderId { get; set; }
    public DxLabCode DxCode { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
}

public class ClaimForInvoiceDto
{
    public Guid Id { get; set; }
    public string? ClaimId { get; set; }
    public DateTime ClaimDate { get; set; }
    public string? ClaimStatus { get; set; }
}


public class AssessmentPlanSnomed
{
    public Guid? Identifier { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? Encounter { get; set; }
    public ChildMedicalCode ChildMedicalCode { get; set; }
    public SnomedCode SnomedCode { get; set; }
    public Guid? AssessmentPlanId { get; set; }
    public Guid? Subject { get; set; }
    public Guid? Performer { get; set; }
    public string? Notes { get; set; }
    public DateTime? OnSetDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsDeleted { get; set; }
}

public class AssessmentPlanCondition
{
    public Guid? Identifier { get; set; }
    public Guid? Encounter { get; set; }
    public Guid? ParentId { get; set; }
    public ChildMedicalCode ChildMedicalCode { get; set; }
    public ConditionCode ConditionCode { get; set; }
    public Guid? AssessmentPlanId { get; set; }
    public Guid? Subject { get; set; }
    public Guid? Performer { get; set; }
    public string? Notes { get; set; }
    public DateTime? OnSetDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? IsPrimary { get; set; }
}

public class ChildMedicalCode
{
    public List<AssessmentPlanCondition> AssessmentPlanCondition { get; set; }

    public List<AssessmentPlanProcedures> AssessmentPlanProcedures { get; set; }

    public List<AssessmentPlanSnomed> AssessmentPlanSnomed { get; set; }

    public List<AssessmentPlanMedication> AssessmentPlanMedication { get; set; }

    public List<RadiologyOrderRequest> RadiologyOrderRequest { get; set; }
}

public class AssessmentPlanProcedures
{
    public Guid? Identifier { get; set; }
    public Guid? Encounter { get; set; }
    public Guid? ParentId { get; set; }
    public ChildMedicalCode ChildMedicalCode { get; set; }
    public ProcedureCode ProcedureCode { get; set; }
    public Guid? AssessmentPlanId { get; set; }
    public Guid? Subject { get; set; }
    public Guid? Performer { get; set; }
    public string? Notes { get; set; }
    public DateTime? OnSetDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsDeleted { get; set; }
}

public class ProcedureCode
{
    public string? System { get; set; }
    public string? Code { get; set; }
    public string? DisplayName { get; set; }
}

public class ConditionCode
{
    public string? System { get; set; }
    public string? Code { get; set; }
    public string? Display { get; set; }
}

public class SnomedCode
{
    public string? Code { get; set; }
    public string? Display { get; set; }
    public string? System { get; set; }
}

public class DxLabCode
{
    public string? Code { get; set; }
    public string? DisplayName { get; set; }
    public string? System { get; set; }
}

public class TestCode
{
    public string? Code { get; set; }
    public string? DisplayName { get; set; }
    public string? System { get; set; }
}