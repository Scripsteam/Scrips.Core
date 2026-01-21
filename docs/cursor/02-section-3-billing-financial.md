# Billing, Invoicing & Claims - Complete Documentation

## OVERVIEW

**Purpose:** Manage pricing, invoice generation, payment processing, and claim submission for healthcare services

**Key Entities:** Invoice, BillingProfile, ProcedureCode, PaymentReceipt, CalculatePriceRequest, PatientCoPayment

**Key Workflows:** Price calculation, invoice generation, payment processing, claim submission

**PHI Scope:** YES - Financial PHI (patient ID in invoices, service dates, diagnosis codes)

---

## ENTITIES

### InvoiceDetailsDto

**Location:** Models/Billing/InvoiceDetailsDto.cs:3

**Purpose:** Invoice line item with service, pricing, sponsor allocation, and diagnosis pointers

**Key Fields (PHI):**
- InvoiceId, ServiceId, ServiceAddedDate (**YES** - linked to patient encounter)
- DXPointer, DxPointers (List) - Diagnosis code pointers (**YES** - clinical data)
- GrossAmount, NetAmount, SponsorAmount, PatientAmount - Pricing breakdown
- ProcedureCode - Service performed (CPT/HCPCS code)
- BillingGroupId - Fee schedule reference
- Quantity, DiscountAmount, DiscountReason
- AuthCode, ValidityStartDate, ValidityEndDate - Prior authorization

**Relationships:**
- Invoice: Many line items to one invoice
- ProcedureCode: Each line item has one procedure code
- Patient: Linked via invoice to patient

---

### CalculatePriceRequest

**Location:** Models/Billing/CalculatePriceRequest.cs:1

**Purpose:** Multi-factor pricing input (service codes, provider, sponsor, patient)

**Key Fields:**
- ServiceCodes (List) - Services being priced
- ProviderId - Provider performing service
- SponsorId - Insurance or corporate payer
- PatientId - For patient-specific pricing
- PracticeId - Facility location affects pricing

**Usage:** POST /api/Billing/Agreements/CalculatePrice (IBillingApi.cs:14)

---

### BillingGroupDto / ProcedureCodeModel

**Location:** Models/Billing/BillingGroupDto.cs, ProcedureCodeModel.cs

**Purpose:** Fee schedules and service codes for billing

**Key Concepts:**
- BillingGroup: Provider/practice fee schedule
- ProcedureCode: CPT, HCPCS codes for services
- Agreement-based pricing: Sponsor-specific rates

---

## WORKFLOWS

### Workflow 1: Multi-Factor Price Calculation

**Entry Point:** IBillingApi.cs:14,20

**Steps:**
1. **Service Selection** - Choose procedure codes
2. **Sponsor Identification** - Insurance or corporate
3. **Billing Profile Lookup** - Provider fee schedule
4. **Agreement Pricing** - Sponsor-specific rates
5. **Calculate Split** - Sponsor share vs patient share
6. **Return FeeSummaryResponse** - Gross, net, sponsor, patient amounts

**File:** IBillingApi.cs:14 (CalculatePrice), 20 (PreCalculatePrice)

**Error Handling:**
- No billing profile: Use default rates or block booking
- Expired agreement: Prompt for sponsor update
- Price calculation failure: Blocks appointment booking

**PHI:** Patient ID, service codes, sponsor details

---

### Workflow 2: Invoice Generation

**Entry Point:** IBillingApi.cs:26

**Steps:**
1. **Service Capture** - From appointment or encounter
2. **Price Calculation** - Call CalculatePrice
3. **Diagnosis Codes** - Link ICD-10 codes (DXPointer)
4. **Line Items** - Create InvoiceDetailsDto for each service
5. **Sponsor Allocation** - Split amounts
6. **Invoice Creation** - Generate invoice document
7. **Email Invoice** - IEmailSenderApi.cs:8

**PHI:** Patient ID, service dates, diagnosis codes

---

### Workflow 3: Payment Processing

**Entry Point:** IBillingApi.cs:29,34,40

**Steps:**
1. **Balance Inquiry** - AvailableBalance endpoint
2. **Payment Collection** - Amount, method
3. **Receipt Generation** - PaymentReceiptResponse
4. **Balance Update** - Reduce outstanding amount
5. **Track Sponsor vs Patient** - Separate accounting

**PHI:** Patient ID, payment amounts

---

### Workflow 4: Claim Submission

**Entry Point:** Inferred (not visible in core library)

**Steps:**
1. **Invoice Finalization** - Lock invoice
2. **Claim Generation** - Standard format (X12 837 or NABIDH)
3. **Code Mapping** - ICD-10 diagnoses, CPT procedures
4. **Payer Submission** - To insurance company
5. **Status Tracking** - Submitted/Accepted/Rejected
6. **Remittance Processing** - Payment from payer

**Standards:** ICD-10, CPT/HCPCS, X12 EDI

---

## INTEGRATIONS

**Billing Service (IBillingApi.cs:6-45):**
- 6 endpoints: CalculatePrice, PreCalculatePrice, Invoice, AvailableBalance, PaymentReceipts (2 variants)
- Purpose: Core billing operations

**Patient Service (IPatientApi.cs:16,22):**
- Sponsor verification for pricing

**Scheduling Service (ISchedulingApi.cs:23):**
- Appointment services for billing

**Email Service (IEmailSenderApi.cs:8):**
- Invoice delivery

---

## COMPLIANCE SUMMARY

**PHI Fields:** Patient ID in invoices, service dates, diagnosis codes (financial PHI)

**Audit:** ✅ All invoice/payment operations logged via AuditableBaseDbContext.cs:25

**Retention:** 7 years for financial records (HIPAA/UAE requirement)

**Accuracy:** Multi-factor pricing must be auditable and reproducible

**Standards:** ICD-10 (diagnoses), CPT/HCPCS (procedures) for claims

---

**Document Version:** 1.0  
**Last Updated:** January 21, 2026  
**Audited Against:** Scrips.Core v7.0 (.NET 7.0)
