using System;
using System.Collections.Generic;
using Scrips.Core.Models.Scheduling;

namespace Scrips.Core.Models.Billing
{
    public class PaymentReceiptResponse
    {
        public Guid Id { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime ReceiptDate { get; set; }
        public double Amount { get; set; }
        public double RecieptAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public Guid LinkedReceiptId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? VoidedDate { get; set; }
        public Guid? EncounterId { get; set; }
        public DateTime? EncounterDate { get; set; }
        public string Status { get; set; }
        public VoidReasonDto VoidReason { get; set; }
        public PatientResponse Patient { get; set; }
        public ProviderResponse Provider { get; set; }
        public string TimeZone { get; set; }
        public OrganizationDto Organization { get; set; }

        public PracticeResponse Practice { get; set; }
        public ExtendUserDto VoidedBy { get; set; }
        public ExtendUserDto ReceivedBy { get; set; }
        public Guid CashierShiftId { get; set; }

        public List<RefundDetailsDto> Refunds { get; set; }
    }

    public class OrganizationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
    }

    public class VoidReasonDto
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
    }

    public class RefundDetailsDto
    {
        public Guid Id { get; set; }
        public string RefundReceiptNumber { get; set; }
        public string RefundMethod { get; set; }
        public string LinkedReceiptNumber { get; set; }
        public double Amount { get; set; }
        public DateTime RefundDate { get; set; }
        public ExtendUserDto RefundedBy { get; set; }
    }

    public class ExtendUserDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Photo { get; set; }

        public Guid OrganisationId { get; set; }
    }
}
